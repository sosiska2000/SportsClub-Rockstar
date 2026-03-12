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
    public class ScheduleViewModel : INotifyPropertyChanged
    {
        private readonly IScheduleService _scheduleService;
        private readonly Action<Page> _navigate;

        private ObservableCollection<Models.Schedule> _schedules = new();
        public ObservableCollection<Models.Schedule> Schedules
        {
            get => _schedules;
            set
            {
                _schedules = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadSchedulesCommand { get; }
        public ICommand AddScheduleCommand { get; }
        public ICommand EditScheduleCommand { get; }
        public ICommand DeleteScheduleCommand { get; }
        public ICommand OpenScheduleDetailsCommand { get; }

        public ScheduleViewModel(IScheduleService scheduleService, Action<Page> navigate)
        {
            _scheduleService = scheduleService;
            _navigate = navigate;

            LoadSchedulesCommand = new AsyncRelayCommand(async () => await LoadSchedules());
            AddScheduleCommand = new RelayCommand(() => AddSchedule());
            EditScheduleCommand = new AsyncRelayCommand<int>(async (id) => await EditSchedule(id));
            DeleteScheduleCommand = new AsyncRelayCommand<int>(async (id) => await DeleteSchedule(id));
            OpenScheduleDetailsCommand = new RelayCommand<int>((id) => OpenScheduleDetails(id));

            LoadSchedulesCommand.Execute(null);
        }

        private async Task LoadSchedules()
        {
            try
            {
                Debug.WriteLine("Loading schedules...");
                var schedules = await _scheduleService.GetGroupSchedulesAsync();
                Debug.WriteLine($"Loaded {schedules.Count} schedules");

                Schedules = new ObservableCollection<Models.Schedule>(schedules);

                foreach (var s in schedules)
                {
                    Debug.WriteLine($"Schedule {s.Id}: {s.DirectionName} - {s.DateTime}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadSchedules error: {ex.Message}");
                MessageBox.Show($"Ошибка загрузки расписания: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddSchedule()
        {
            _navigate(new EditScheduleView(_navigate, null));
        }

        private async Task EditSchedule(int id)
        {
            try
            {
                var schedule = await _scheduleService.GetScheduleByIdAsync(id);
                if (schedule != null)
                {
                    _navigate(new EditScheduleView(_navigate, schedule));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EditSchedule error: {ex.Message}");
                MessageBox.Show($"Ошибка при редактировании: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteSchedule(int id)
        {
            try
            {
                var result = MessageBox.Show(
                    "Вы уверены, что хотите удалить это занятие?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var success = await _scheduleService.DeleteScheduleAsync(id);

                    if (success)
                    {
                        var schedule = Schedules.FirstOrDefault(s => s.Id == id);
                        if (schedule != null)
                        {
                            Schedules.Remove(schedule);
                        }

                        MessageBox.Show("Занятие успешно удалено!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении занятия!", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeleteSchedule error: {ex.Message}");
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenScheduleDetails(int id)
        {
            _navigate(new ScheduleDetailsView(_navigate, id));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}