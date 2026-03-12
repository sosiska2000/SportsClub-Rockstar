using Microsoft.Extensions.DependencyInjection;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Schedule;
using Rockstar.Admin.WPF.Views.Main;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rockstar.Admin.WPF.Views.Schedule
{
    public partial class ScheduleView : Page
    {
        private readonly ScheduleViewModel _viewModel;
        private readonly Action<Page> _navigate;

        public ScheduleView(Action<Page> navigate)
        {
            try
            {
                Debug.WriteLine("ScheduleView: Initializing...");
                _navigate = navigate;

                InitializeComponent();

                var scheduleService = App.Services.GetRequiredService<IScheduleService>();
                Debug.WriteLine("ScheduleView: ScheduleService obtained");

                _viewModel = new ScheduleViewModel(scheduleService, navigate);
                Debug.WriteLine("ScheduleView: ViewModel created");

                DataContext = _viewModel;
                Debug.WriteLine("ScheduleView: DataContext set");

                this.Loaded += (s, e) =>
                {
                    Debug.WriteLine("ScheduleView: Loaded event fired");
                    _viewModel.LoadSchedulesCommand.Execute(null);
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ScheduleView constructor error: {ex.Message}");
                MessageBox.Show($"Ошибка инициализации страницы: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ScheduleCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is int scheduleId)
            {
                _viewModel.OpenScheduleDetailsCommand.Execute(scheduleId);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _navigate(new MainPage(_navigate));
        }
    }
}