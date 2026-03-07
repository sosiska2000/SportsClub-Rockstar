using Microsoft.Extensions.DependencyInjection;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Directions;
using Rockstar.Admin.WPF.Views.Main;
using System;
using System.Windows.Controls;

namespace Rockstar.Admin.WPF.Views.Directions
{
    public partial class DirectionDetailView : Page
    {
        private readonly Action<Page> _navigate;

        public DirectionDetailView(Action<Page> navigate, string directionKey)
        {
            InitializeComponent();

            // Сохраняем navigate для использования в других методах
            _navigate = navigate;

            var directionService = App.Services.GetRequiredService<IDirectionService>();
            var serviceService = App.Services.GetRequiredService<IServiceService>();
            var serviceTypeService = App.Services.GetRequiredService<IServiceTypeService>();

            var viewModel = new DirectionDetailViewModel(
                directionService,
                serviceService,
                serviceTypeService,
                navigate,
                directionKey);

            DataContext = viewModel;
        }

        private void LogoutButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _navigate(new DirectionsView(_navigate));
        }
    }
}