using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rockstar.Admin.WPF.ViewModels.Auth
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly Action<Page> _navigate;  // 🔑 Action<Page>

        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoggingIn;

        public LoginViewModel(IAuthService authService, Action<Page> navigate)
        {
            _authService = authService;
            _navigate = navigate;
        }

        public string Email
        {
            get => _email;
            set
            {
                if (SetField(ref _email, value))
                {
                    OnPropertyChanged(nameof(CanLogin));
                    ClearError();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetField(ref _password, value))
                {
                    OnPropertyChanged(nameof(CanLogin));
                    ClearError();
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set => SetField(ref _errorMessage, value);
        }

        public bool IsLoggingIn
        {
            get => _isLoggingIn;
            private set
            {
                if (SetField(ref _isLoggingIn, value))
                {
                    OnPropertyChanged(nameof(CanLogin));
                    OnPropertyChanged(nameof(IsLoadingVisible));
                }
            }
        }

        public Visibility IsErrorVisible => string.IsNullOrEmpty(ErrorMessage) ? Visibility.Collapsed : Visibility.Visible;
        public Visibility IsLoadingVisible => IsLoggingIn ? Visibility.Visible : Visibility.Collapsed;
        public bool CanLogin => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password) && !IsLoggingIn;

        // 🔑 RelayCommand без параметра
        public ICommand LoginCommand => new RelayCommand(async () => await ExecuteLogin(), () => CanLogin);
        public ICommand ForgotPasswordCommand => new RelayCommand(() => ExecuteForgotPassword());

        private void ClearError()
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
                ErrorMessage = string.Empty;
        }

        private async Task ExecuteLogin()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Заполните все поля";
                OnPropertyChanged(nameof(IsErrorVisible));
                return;
            }

            if (!Email.Contains("@") || !Email.Contains("."))
            {
                ErrorMessage = "Некорректный формат email";
                OnPropertyChanged(nameof(IsErrorVisible));
                return;
            }

            IsLoggingIn = true;
            ErrorMessage = string.Empty;

            try
            {
                var result = await _authService.LoginAsync(Email, Password);

                if (result.Success && result.User != null)
                {
                    _navigate(new Views.Main.MainPage(_navigate));
                }
                else
                {
                    ErrorMessage = result.Message ?? "Ошибка авторизации";
                    OnPropertyChanged(nameof(IsErrorVisible));
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Ошибка подключения к серверу";
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
            }
            finally
            {
                IsLoggingIn = false;
            }
        }

        private void ExecuteForgotPassword()
        {
            MessageBox.Show(
                "Функция восстановления пароля будет доступна в следующей версии.",
                "Информация",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}