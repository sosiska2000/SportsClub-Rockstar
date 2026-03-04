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

        public LoginPage(Action<Page> navigate)
        {
            InitializeComponent();

            // Получаем сервисы из App.Services напрямую
            var authService = App.Services.GetRequiredService<IAuthService>();

            // Создаём ViewModel вручную
            _viewModel = new LoginViewModel(authService, navigate);

            DataContext = _viewModel;

            PasswordBox.PasswordChanged += (s, e) => _viewModel.Password = PasswordBox.Password;
        }
    }
}