using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Commands;
using Rockstar.Admin.WPF.Views.Clients;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Rockstar.Admin.WPF.ViewModels.Clients
{
    public class AddClientViewModel : INotifyPropertyChanged
    {
        private readonly IClientService _clientService;
        private readonly Action<Page> _navigate;
        private readonly Client? _editingClient;

        private string _pageTitle;
        public string PageTitle
        {
            get => _pageTitle;
            set { _pageTitle = value; OnPropertyChanged(); }
        }

        private string _saveButtonText;
        public string SaveButtonText
        {
            get => _saveButtonText;
            set { _saveButtonText = value; OnPropertyChanged(); }
        }

        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); ValidateForm(); }
        }

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); ValidateForm(); }
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); ValidateForm(); }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); ValidateForm(); }
        }

        private string _phone = string.Empty;
        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(); }
        }

        private string _age = string.Empty;
        public string Age
        {
            get => _age;
            set { _age = value; OnPropertyChanged(); }
        }

        private byte[]? _photo;
        public byte[]? Photo
        {
            get => _photo;
            set { _photo = value; OnPropertyChanged(); LoadPhotoPreview(value); }
        }

        private bool _canSave;
        public bool CanSave
        {
            get => _canSave;
            set { _canSave = value; OnPropertyChanged(); }
        }

        private string _firstNameError = string.Empty;
        public string FirstNameError
        {
            get => _firstNameError;
            set { _firstNameError = value; OnPropertyChanged(); }
        }

        private string _lastNameError = string.Empty;
        public string LastNameError
        {
            get => _lastNameError;
            set { _lastNameError = value; OnPropertyChanged(); }
        }

        private string _emailError = string.Empty;
        public string EmailError
        {
            get => _emailError;
            set { _emailError = value; OnPropertyChanged(); }
        }

        private string _passwordError = string.Empty;
        public string PasswordError
        {
            get => _passwordError;
            set { _passwordError = value; OnPropertyChanged(); }
        }

        private BitmapImage? _photoPreview;
        public BitmapImage? PhotoPreview
        {
            get => _photoPreview;
            set { _photoPreview = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddClientViewModel(IClientService clientService, Action<Page> navigate, Client? client)
        {
            _clientService = clientService;
            _navigate = navigate;
            _editingClient = client;

            SaveCommand = new AsyncRelayCommand(async () => await SaveAsync(), () => CanSave);
            CancelCommand = new RelayCommand(() => Cancel());

            if (client != null)
            {
                PageTitle = "Редактирование клиента";
                SaveButtonText = "Сохранить изменения";

                FirstName = client.FirstName;
                LastName = client.LastName;
                Email = client.Email;
                Phone = client.Phone ?? string.Empty;
                Age = client.Age?.ToString() ?? string.Empty;
                Photo = client.Photo;

                if (client.Photo != null && client.Photo.Length > 0)
                {
                    LoadPhotoPreview(client.Photo);
                }
            }
            else
            {
                PageTitle = "Новый клиент";
                SaveButtonText = "Создать клиента";
            }

            ValidateForm();
        }

        private void LoadPhotoPreview(byte[]? photoBytes)
        {
            if (photoBytes != null && photoBytes.Length > 0)
            {
                try
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = new MemoryStream(photoBytes);
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    image.Freeze();
                    PhotoPreview = image;
                }
                catch
                {
                    PhotoPreview = null;
                }
            }
            else
            {
                PhotoPreview = null;
            }
        }

        private void ValidateForm()
        {
            FirstNameError = string.Empty;
            LastNameError = string.Empty;
            EmailError = string.Empty;
            PasswordError = string.Empty;

            bool isValid = true;

            if (string.IsNullOrWhiteSpace(FirstName))
            {
                FirstNameError = "Имя обязательно";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                LastNameError = "Фамилия обязательна";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                EmailError = "Email обязателен";
                isValid = false;
            }
            else if (!IsValidEmail(Email))
            {
                EmailError = "Некорректный email";
                isValid = false;
            }

            if (_editingClient == null && string.IsNullOrWhiteSpace(Password))
            {
                PasswordError = "Пароль обязателен";
                isValid = false;
            }

            CanSave = isValid;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private async System.Threading.Tasks.Task SaveAsync()
        {
            if (!CanSave) return;

            try
            {
                var client = new Client
                {
                    Id = _editingClient?.Id ?? 0,
                    FirstName = FirstName?.Trim() ?? "",
                    LastName = LastName?.Trim() ?? "",
                    Email = Email?.Trim().ToLower() ?? "",
                    PasswordHash = _editingClient?.PasswordHash ?? Password,
                    Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim(),
                    Age = int.TryParse(Age, out var age) ? age : null,
                    Photo = Photo,
                    IsActive = true
                };

                Debug.WriteLine($"=== SAVE CLIENT DEBUG ===");
                Debug.WriteLine($"FirstName: '{client.FirstName}'");
                Debug.WriteLine($"LastName: '{client.LastName}'");
                Debug.WriteLine($"Email: '{client.Email}'");
                Debug.WriteLine($"=========================");

                bool success;

                if (_editingClient != null)
                {
                    success = await _clientService.UpdateAsync(client);
                }
                else
                {
                    success = await _clientService.CreateAsync(client);
                }

                if (success)
                {
                    MessageBox.Show(
                        _editingClient != null ? "Клиент успешно обновлен!" : "Клиент успешно создан!",
                        "Успех",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    var clientsView = new ClientsView(_navigate);
                    _navigate(clientsView);
                }
                else
                {
                    var error = _editingClient != null
                        ? "Ошибка при обновлении клиента! Возможно, email уже используется."
                        : "Ошибка при создании клиента! Возможно, email уже используется.";

                    MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            _navigate(new ClientsView(_navigate));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}