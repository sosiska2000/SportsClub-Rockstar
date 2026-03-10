using Microsoft.Extensions.DependencyInjection;
using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Schedule;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Rockstar.Admin.WPF.Views.Schedule
{
    public partial class EditScheduleView : Page
    {
        private readonly EditScheduleViewModel _viewModel;

        public EditScheduleView(Action<Page> navigate, Models.Schedule? schedule)
        {
            try
            {
                Debug.WriteLine("EditScheduleView: Initializing...");

                InitializeComponent();

                var scheduleService = App.Services.GetRequiredService<IScheduleService>();
                Debug.WriteLine("EditScheduleView: ScheduleService obtained");

                _viewModel = new EditScheduleViewModel(scheduleService, navigate, schedule);
                Debug.WriteLine("EditScheduleView: ViewModel created");

                DataContext = _viewModel;
                Debug.WriteLine("EditScheduleView: DataContext set");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EditScheduleView constructor error: {ex.Message}");
                MessageBox.Show($"Ошибка инициализации страницы: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}