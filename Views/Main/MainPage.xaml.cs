using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Controls;

namespace Rockstar.Admin.WPF.Views.Main
{
    public partial class MainPage : Page
    {
        public MainPage(Action<Page> navigate)
        {
            InitializeComponent();

            var authService = App.Services.GetRequiredService<Services.Interfaces.IAuthService>();
            var viewModel = new ViewModels.Main.MainViewModel(authService, navigate);

            DataContext = viewModel;
        }
    }
}