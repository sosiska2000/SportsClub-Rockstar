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

            _viewModel = new LoginViewModel(
                services.GetRequiredService<IAuthService>(),
                navigate);

            DataContext = _viewModel;

            // Устанавливаем начальный пароль из ViewModel в PasswordBox
            PasswordBox.Password = _viewModel.Password;
        }

        // Обработчик для передачи пароля из PasswordBox в ViewModel
        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}