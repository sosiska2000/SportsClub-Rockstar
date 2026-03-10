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
    public class EditScheduleViewModel : INotifyPropertyChanged
    {
        private readonly IScheduleService _scheduleService;
        private readonly Action<Page> _navigate;
        private readonly Models.Schedule? _editingSchedule;

        private string _pageTitle;
        public string PageTitle
        {
            get => _pageTitle;
            set { _pageTitle = value; OnPropertyChanged(); }
        }

        private string _saveButtonText;
        public string SaveButtonText
        {
            get => _saveButtonText;
            set { _saveButtonText = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Direction> _directions = new();
        public ObservableCollection<Direction> Directions
        {
            get => _directions;
            set { _directions = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Service> _services = new();
        public ObservableCollection<Service> Services
        {
            get => _services;
            set { _services = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Trainer> _trainers = new();
        public ObservableCollection<Trainer> Trainers
        {
            get => _trainers;
            set { _trainers = value; OnPropertyChanged(); }
        }

        private Direction? _selectedDirection;
        public Direction? SelectedDirection
        {
            get => _selectedDirection;
            set
            {
                _selectedDirection = value;
                OnPropertyChanged();
                if (value != null)
                {
                    LoadServicesAsync(value.Id);
                }
                else
                {
                    Services.Clear();
                }
                ValidateForm();
            }
        }

        private Service? _selectedService;
        public Service? SelectedService
        {
            get => _selectedService;
            set
            {
                _selectedService = value;
                OnPropertyChanged();
                if (value != null)
                {
                    // Автоматически подставляем цену из услуги
                    Price = value.Price;
                }
                ValidateForm();
            }
        }

        private Trainer? _selectedTrainer;
        public Trainer? SelectedTrainer
        {
            get => _selectedTrainer;
            set
            {
                _selectedTrainer = value;
                OnPropertyChanged();
                ValidateForm();
            }
        }

        private DateTime _date = DateTime.Today;
        public DateTime Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(); ValidateForm(); }
        }

        private string _time = "10:00";
        public string Time
        {
            get => _time;
            set { _time = value; OnPropertyChanged(); ValidateForm(); }
        }

        private int _durationMinutes = 60;
        public int DurationMinutes
        {
            get => _durationMinutes;
            set { _durationMinutes = value; OnPropertyChanged(); ValidateForm(); }
        }

        private int _maxParticipants = 20;
        public int MaxParticipants
        {
            get => _maxParticipants;
            set { _maxParticipants = value; OnPropertyChanged(); ValidateForm(); }
        }

        private decimal _price = 0;
        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(); ValidateForm(); }
        }

        private bool _canSave;
        public bool CanSave
        {
            get => _canSave;
            set { _canSave = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public EditScheduleViewModel(IScheduleService scheduleService, Action<Page> navigate, Models.Schedule? schedule)
        {
            _scheduleService = scheduleService;
            _navigate = navigate;
            _editingSchedule = schedule;

            SaveCommand = new AsyncRelayCommand(async () => await SaveAsync(), () => CanSave);
            CancelCommand = new RelayCommand(() => Cancel());

            InitializeAsync();

            if (schedule != null)
            {
                PageTitle = "Редактирование занятия";
                SaveButtonText = "Сохранить изменения";
                LoadScheduleData(schedule);
            }
            else
            {
                PageTitle = "Новое занятие";
                SaveButtonText = "Создать занятие";
            }
        }

        private async void InitializeAsync()
        {
            await LoadDirections();
            await LoadTrainers();
        }

        private async Task LoadDirections()
        {
            try
            {
                var directions = await _scheduleService.GetDirectionsAsync();
                Directions = new ObservableCollection<Direction>(directions);
                Debug.WriteLine($"Loaded {directions.Count} directions");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadDirections error: {ex.Message}");
                MessageBox.Show($"Ошибка загрузки направлений: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadServicesAsync(int directionId)
        {
            try
            {
                var services = await _scheduleService.GetServicesByDirectionAsync(directionId);
                Services = new ObservableCollection<Service>(services);
                Debug.WriteLine($"Loaded {services.Count} services for direction {directionId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadServicesAsync error: {ex.Message}");
            }
        }

        private async Task LoadTrainers()
        {
            try
            {
                var trainers = await _scheduleService.GetTrainersAsync();
                Trainers = new ObservableCollection<Trainer>(trainers);
                Debug.WriteLine($"Loaded {trainers.Count} trainers");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadTrainers error: {ex.Message}");
                MessageBox.Show($"Ошибка загрузки тренеров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadScheduleData(Models.Schedule schedule)
        {
            SelectedDirection = Directions.FirstOrDefault(d => d.Id == schedule.DirectionId);

            // Загружаем услуги для этого направления
            if (SelectedDirection != null)
            {
                Task.Run(async () => await LoadServicesAsync(SelectedDirection.Id))
                    .ContinueWith(t =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            SelectedService = Services.FirstOrDefault(s => s.Id == schedule.ServiceId);
                        });
                    });
            }

            SelectedTrainer = Trainers.FirstOrDefault(t => t.Id == schedule.TrainerId);

            Date = schedule.DateTime.Date;
            Time = schedule.DateTime.ToString("HH:mm");
            DurationMinutes = schedule.DurationMinutes;
            MaxParticipants = schedule.MaxParticipants;
            Price = schedule.Price;
        }

        private void ValidateForm()
        {
            bool isValid = true;

            if (SelectedDirection == null)
                isValid = false;

            if (SelectedService == null)
                isValid = false;

            if (SelectedTrainer == null)
                isValid = false;

            if (string.IsNullOrWhiteSpace(Time))
                isValid = false;

            if (DurationMinutes <= 0)
                isValid = false;

            if (MaxParticipants <= 0)
                isValid = false;

            if (Price <= 0)
                isValid = false;

            CanSave = isValid;
        }

        private DateTime CombineDateAndTime()
        {
            if (TimeSpan.TryParse(Time, out var timeSpan))
            {
                return Date.Date.Add(timeSpan);
            }
            return Date;
        }

        private async Task SaveAsync()
        {
            if (!CanSave) return;

            try
            {
                // Объединяем дату и время
                var dateTime = CombineDateAndTime();

                var schedule = new Models.Schedule
                {
                    Id = _editingSchedule?.Id ?? 0,
                    DirectionId = SelectedDirection?.Id ?? 0,
                    DirectionName = SelectedDirection?.Name ?? "",
                    ServiceId = SelectedService?.Id,
                    ServiceName = SelectedService?.Name ?? "",
                    TrainerId = SelectedTrainer?.Id,
                    TrainerName = SelectedTrainer?.FullName ?? "",
                    DateTime = dateTime,
                    DurationMinutes = DurationMinutes,
                    MaxParticipants = MaxParticipants,
                    Price = Price,
                    IsGroup = true,
                    IsActive = true
                };

                Debug.WriteLine($"=== SAVING SCHEDULE ===");
                Debug.WriteLine($"Id: {schedule.Id}");
                Debug.WriteLine($"DirectionId: {schedule.DirectionId}");
                Debug.WriteLine($"ServiceId: {schedule.ServiceId}");
                Debug.WriteLine($"TrainerId: {schedule.TrainerId}");
                Debug.WriteLine($"DateTime: {schedule.DateTime}");
                Debug.WriteLine($"DurationMinutes: {schedule.DurationMinutes}");
                Debug.WriteLine($"MaxParticipants: {schedule.MaxParticipants}");
                Debug.WriteLine($"Price: {schedule.Price}");
                Debug.WriteLine($"=======================");

                bool success;

                if (_editingSchedule != null)
                {
                    success = await _scheduleService.UpdateScheduleAsync(schedule);
                }
                else
                {
                    success = await _scheduleService.CreateScheduleAsync(schedule);
                }

                if (success)
                {
                    MessageBox.Show(
                        _editingSchedule != null ? "Занятие успешно обновлено!" : "Занятие успешно создано!",
                        "Успех",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    _navigate(new ScheduleView(_navigate));
                }
                else
                {
                    MessageBox.Show("Ошибка при сохранении занятия! Проверьте правильность введенных данных.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SaveAsync error: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
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