using Microsoft.Extensions.DependencyInjection;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Subscriptions;
using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rockstar.Admin.WPF.Views.Subscriptions
{
    public partial class AddSubscriptionView : Page
    {
        private readonly AddSubscriptionViewModel _viewModel;

        public AddSubscriptionView(Action<Page> navigate, Models.Subscription? subscription)
        {
            InitializeComponent();

            var subscriptionService = App.Services.GetRequiredService<ISubscriptionService>();
            var directionService = App.Services.GetRequiredService<IDirectionService>();

            _viewModel = new AddSubscriptionViewModel(
                subscriptionService,
                directionService,
                navigate,
                subscription);

            DataContext = _viewModel;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}