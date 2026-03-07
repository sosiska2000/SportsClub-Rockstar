using Microsoft.Extensions.DependencyInjection;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Directions;
using Rockstar.Admin.WPF.Views.Main;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rockstar.Admin.WPF.Views.Directions
{
    public partial class DirectionsView : Page
    {
        private readonly Action<Page> _navigate;

        public DirectionsView(Action<Page> navigate)
        {
            InitializeComponent();
            _navigate = navigate;

            var directionService = App.Services.GetRequiredService<IDirectionService>();
            var viewModel = new DirectionsViewModel(directionService, navigate);

            DataContext = viewModel;
        }

        private void DirectionCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is string directionKey)
            {
                _navigate(new DirectionDetailView(_navigate, directionKey));
            }
        }

        private void backbutton_click(object sender, System.Windows.RoutedEventArgs e)
        {
            _navigate(new MainPage(_navigate));
        }
    }
}