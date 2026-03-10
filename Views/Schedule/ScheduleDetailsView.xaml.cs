using Microsoft.Extensions.DependencyInjection;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Schedule;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Rockstar.Admin.WPF.Views.Schedule
{
    public partial class ScheduleDetailsView : Page
    {
        private readonly ScheduleDetailsViewModel _viewModel;

        public ScheduleDetailsView(Action<Page> navigate, int scheduleId)
        {
            try
            {
                Debug.WriteLine($"ScheduleDetailsView: Initializing for schedule ID={scheduleId}");

                InitializeComponent();

                var scheduleService = App.Services.GetRequiredService<IScheduleService>();
                var clientService = App.Services.GetRequiredService<IClientService>();

                _viewModel = new ScheduleDetailsViewModel(scheduleService, clientService, navigate, scheduleId);
                DataContext = _viewModel;

                this.Loaded += async (s, e) =>
                {
                    Debug.WriteLine("ScheduleDetailsView: Loaded event fired");
                    await _viewModel.LoadDataAsync();
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ScheduleDetailsView constructor error: {ex.Message}");
                MessageBox.Show($"Ошибка инициализации страницы: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}