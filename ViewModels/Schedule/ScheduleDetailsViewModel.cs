using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Commands;
using Rockstar.Admin.WPF.Views.Schedule;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rockstar.Admin.WPF.ViewModels.Schedule
{
    public class ScheduleDetailsViewModel : INotifyPropertyChanged
    {
        private readonly IScheduleService _scheduleService;
        private readonly IClientService _clientService;
        private readonly Action<Page> _navigate;
        private readonly int _scheduleId;

        private Models.Schedule? _schedule;
        public Models.Schedule? Schedule
        {
            get => _schedule;
            set
            {
                _schedule = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasNoEnrollments));
            }
        }

        private ObservableCollection<Enrollment> _enrollments = new();
        public ObservableCollection<Enrollment> Enrollments
        {
            get => _enrollments;
            set
            {
                _enrollments = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasNoEnrollments));
            }
        }

        private ObservableCollection<Client> _availableClients = new();
        public ObservableCollection<Client> AvailableClients
        {
            get => _availableClients;
            set
            {
                _availableClients = value;
                OnPropertyChanged();
            }
        }

        private Client? _selectedClient;
        public Client? SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanAddClient));
            }
        }

        public bool HasNoEnrollments => Enrollments == null || Enrollments.Count == 0;
        public bool CanAddClient => SelectedClient != null;

        public ICommand AddClientCommand { get; }
        public ICommand RemoveClientCommand { get; }
        public ICommand EditScheduleCommand { get; }
        public ICommand BackCommand { get; }

        public ScheduleDetailsViewModel(
            IScheduleService scheduleService,
            IClientService clientService,
            Action<Page> navigate,
            int scheduleId)
        {
            _scheduleService = scheduleService;
            _clientService = clientService;
            _navigate = navigate;
            _scheduleId = scheduleId;

            AddClientCommand = new AsyncRelayCommand(async () => await AddClient(), () => CanAddClient);
            RemoveClientCommand = new AsyncRelayCommand<int>(async (enrollmentId) => await RemoveClient(enrollmentId));
            EditScheduleCommand = new RelayCommand(() => EditSchedule());
            BackCommand = new RelayCommand(() => Back());
        }

        public async Task LoadDataAsync()
        {
            try
            {
                Debug.WriteLine($"Loading data for schedule ID={_scheduleId}");

                Schedule = await _scheduleService.GetScheduleByIdAsync(_scheduleId);

                var enrollments = await _scheduleService.GetEnrollmentsByScheduleIdAsync(_scheduleId);
                Enrollments = new ObservableCollection<Enrollment>(enrollments);

                var availableClients = await _scheduleService.GetAvailableClientsAsync(_scheduleId);
                AvailableClients = new ObservableCollection<Client>(availableClients);

                Debug.WriteLine($"Loaded: Schedule={Schedule != null}, Enrollments={Enrollments.Count}, Available={AvailableClients.Count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadDataAsync error: {ex.Message}");
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task AddClient()
        {
            if (SelectedClient == null) return;

            try
            {
                var success = await _scheduleService.AddClientToScheduleAsync(_scheduleId, SelectedClient.Id);

                if (success)
                {
                    // Обновляем данные
                    await LoadDataAsync();
                    SelectedClient = null;

                    MessageBox.Show("Клиент успешно добавлен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении клиента!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AddClient error: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task RemoveClient(int enrollmentId)
        {
            try
            {
                var result = MessageBox.Show(
                    "Удалить клиента из записи?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var success = await _scheduleService.RemoveClientFromScheduleAsync(enrollmentId);

                    if (success)
                    {
                        await LoadDataAsync();

                        MessageBox.Show("Клиент удален из записи!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении клиента!", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RemoveClient error: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditSchedule()
        {
            if (Schedule != null)
            {
                _navigate(new EditScheduleView(_navigate, Schedule));
            }
        }

        private void Back()
        {
            _navigate(new ScheduleView(_navigate));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}