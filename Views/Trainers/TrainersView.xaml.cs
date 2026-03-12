using Microsoft.Extensions.DependencyInjection;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Trainers;
using Rockstar.Admin.WPF.Views.Main;
using System;
using System.Windows.Controls;

namespace Rockstar.Admin.WPF.Views.Trainers
{
    public partial class TrainersView : Page
    {
        private readonly Action<Page> _navigate;
        public TrainersView(Action<Page> navigate)
        {
            InitializeComponent();

            var trainerService = App.Services.GetRequiredService<ITrainerService>();
            var viewModel = new TrainersViewModel(trainerService, navigate);

            DataContext = viewModel;
            _navigate = navigate;
        }

        private void LogoutButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _navigate(new MainPage(_navigate));
        }
    }
}