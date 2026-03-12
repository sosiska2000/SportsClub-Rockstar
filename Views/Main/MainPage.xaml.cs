using Microsoft.Extensions.DependencyInjection;
using Rockstar.Admin.WPF.Views.Auth;
using Rockstar.Admin.WPF.Views.Clients;
using Rockstar.Admin.WPF.Views.Directions;
using Rockstar.Admin.WPF.Views.Schedule;
using Rockstar.Admin.WPF.Views.Subscriptions;
using Rockstar.Admin.WPF.Views.Trainers;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Rockstar.Admin.WPF.Views.Main
{
    public partial class MainPage : Page
    {
        private readonly Action<Page> _navigate;

        public MainPage(Action<Page> navigate)
        {
            InitializeComponent();
            _navigate = navigate;
        }

        private void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            _navigate(new ClientsView(_navigate));
        }


        private void SubscriptionsButton_Click(object sender, RoutedEventArgs e)
        {
            _navigate(new Views.Subscriptions.SubscriptionsView(_navigate));
        }

        private void TrainersButton_Click(object sender, RoutedEventArgs e)
        {
            _navigate(new TrainersView(_navigate));
        }

        private void DirectionsButton_Click(object sender, RoutedEventArgs e)
        {
            _navigate(new DirectionsView(_navigate));
        }

        private void ScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            _navigate(new ScheduleView(_navigate));
        }

        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Вы действительно хотите выйти из системы?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var authService = App.Services.GetRequiredService<Services.Interfaces.IAuthService>();
                await authService.LogoutAsync();
                _navigate(new LoginPage(_navigate));
            }
        }
    }
}