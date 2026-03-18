using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Base;
using Rockstar.Admin.WPF.ViewModels.Commands;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rockstar.Admin.WPF.ViewModels.Auth
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly Action<Page> _navigate;

        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoggingIn;

        public LoginViewModel(IAuthService authService, Action<Page> navigate)
        {
            _authService = authService;
            _navigate = navigate;

            // Для теста - заполняем данные админа
            Email = "adm@mail.ru";
            Password = "qweqwe";

            Debug.WriteLine("=== LoginViewModel Created ===");
            Debug.WriteLine($"Initial Email: '{Email}'");
            Debug.WriteLine($"Initial Password: '{Password}'");
            Debug.WriteLine($"CanLogin: {CanLogin}");
        }

        public string Email
        {
            get => _email;
            set
            {
                if (SetField(ref _email, value))
                {
                    Debug.WriteLine($"Email changed to: '{value}'");
                    Debug.WriteLine($"Email null or whitespace: {string.IsNullOrWhiteSpace(value)}");
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
                    Debug.WriteLine($"Password changed to: '{value}'");
                    Debug.WriteLine($"Password null or whitespace: {string.IsNullOrWhiteSpace(value)}");
                    OnPropertyChanged(nameof(CanLogin));
                    ClearError();
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set
            {
                Debug.WriteLine($"ErrorMessage set to: '{value}'");
                SetField(ref _errorMessage, value);
                OnPropertyChanged(nameof(IsErrorVisible));
            }
        }

        public bool IsLoggingIn
        {
            get => _isLoggingIn;
            private set
            {
                Debug.WriteLine($"IsLoggingIn changed to: {value}");
                if (SetField(ref _isLoggingIn, value))
                {
                    OnPropertyChanged(nameof(CanLogin));
                    OnPropertyChanged(nameof(IsLoadingVisible));
                }
            }
        }

        public Visibility IsErrorVisible =>
            string.IsNullOrEmpty(ErrorMessage) ? Visibility.Collapsed : Visibility.Visible;

        public Visibility IsLoadingVisible =>
            IsLoggingIn ? Visibility.Visible : Visibility.Collapsed;

        public bool CanLogin
        {
            get
            {
                var result = !string.IsNullOrWhiteSpace(Email) &&
                            !string.IsNullOrWhiteSpace(Password) &&
                            !IsLoggingIn;

                Debug.WriteLine($"CanLogin: {result} (Email: '{Email}', Password: '{Password}', IsLoggingIn: {IsLoggingIn})");
                return result;
            }
        }

        public ICommand LoginCommand => new AsyncRelayCommand(ExecuteLogin, () => CanLogin);
        public ICommand ForgotPasswordCommand => new RelayCommand(ExecuteForgotPassword, () => !IsLoggingIn);

        private void ClearError()
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
                ErrorMessage = string.Empty;
        }

        private async Task ExecuteLogin()
        {
            Debug.WriteLine("=== ExecuteLogin START ===");
            Debug.WriteLine($"Email: '{Email}'");
            Debug.WriteLine($"Password: '{Password}'");
            Debug.WriteLine($"IsLoggingIn: {IsLoggingIn}");
            Debug.WriteLine($"CanLogin: {CanLogin}");

            if (!CanLogin)
            {
                Debug.WriteLine("Cannot login - returning");
                return;
            }

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Заполните все поля";
                Debug.WriteLine($"Validation failed: {ErrorMessage}");
                return;
            }

            if (!Email.Contains("@") || !Email.Contains("."))
            {
                ErrorMessage = "Некорректный формат email";
                Debug.WriteLine($"Validation failed: {ErrorMessage}");
                return;
            }

            IsLoggingIn = true;
            ErrorMessage = string.Empty;

            try
            {
                Debug.WriteLine("Calling _authService.LoginAsync...");
                var result = await _authService.LoginAsync(Email, Password);

                Debug.WriteLine($"Login result: Success={result.Success}, Message={result.Message}");

                if (result.Success)
                {
                    Debug.WriteLine("Navigating to MainPage...");
                    _navigate(new Views.Main.MainPage(_navigate));
                }
                else
                {
                    ErrorMessage = result.Message ?? "Ошибка авторизации";
                    Debug.WriteLine($"Login failed: {ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Ошибка подключения. Проверьте соединение.";
                Debug.WriteLine($"Exception: {ex.Message}");
                Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }
            finally
            {
                IsLoggingIn = false;
                Debug.WriteLine("=== ExecuteLogin END ===");
            }
        }

        private void ExecuteForgotPassword()
        {
            MessageBox.Show(
                "Функция восстановления пароля будет доступна в следующей версии.\n\n" +
                "Для входа используйте:\n" +
                "Email: adm@mail.ru\n" +
                "Пароль: qweqwe",
                "Информация",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}