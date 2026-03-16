using Microsoft.Extensions.DependencyInjection;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Auth;
using System;
using System.Windows.Controls;

namespace Rockstar.Admin.WPF.Views.Auth
{
    public partial class LoginPage : Page
    {
        private readonly LoginViewModel _viewModel;

        public LoginPage(IServiceProvider services, Action<Page> navigate)
        {
            InitializeComponent();

            // Создаем ViewModel с правильными параметрами
            _viewModel = new LoginViewModel(
                services.GetRequiredService<IAuthService>(),
                navigate);

            DataContext = _viewModel;
        }
    }
}