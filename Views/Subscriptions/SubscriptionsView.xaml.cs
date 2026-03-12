using Microsoft.Extensions.DependencyInjection;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Subscriptions;
using Rockstar.Admin.WPF.Views.Main;
using System;
using System.Windows.Controls;

namespace Rockstar.Admin.WPF.Views.Subscriptions
{
    public partial class SubscriptionsView : Page
    {
        private readonly Action<Page> _navigate;

        public SubscriptionsView(Action<Page> navigate)
        {
            InitializeComponent();

            // Сохраняем navigate для использования в других методах
            _navigate = navigate;

            var subscriptionService = App.Services.GetRequiredService<ISubscriptionService>();
            var directionService = App.Services.GetRequiredService<IDirectionService>();

            var viewModel = new SubscriptionsViewModel(
                subscriptionService,
                directionService,
                navigate);

            DataContext = viewModel;
        }

        private void LogoutButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _navigate(new MainPage(_navigate));
        }
    }
}