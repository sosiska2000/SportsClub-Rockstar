using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Base;
using Rockstar.Admin.WPF.ViewModels.Commands;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rockstar.Admin.WPF.ViewModels.Trainers
{
    public class AddTrainerViewModel : ViewModelBase
    {
        private readonly ITrainerService _trainerService;
        private readonly Action<Page> _navigate;
        private readonly Trainer? _editingTrainer;

        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _description = string.Empty;
        private int _experience;
        private string _selectedDirection = string.Empty;
        private byte[]? _photo;
        private string _firstNameError = string.Empty;
        private string _lastNameError = string.Empty;
        private string _emailError = string.Empty;
        private string _passwordError = string.Empty;

        // Флаг для отслеживания, был ли изменен пароль
        private bool _isPasswordChanged = false;

        public AddTrainerViewModel(ITrainerService trainerService, Action<Page> navigate, Trainer? trainer)
        {
            _trainerService = trainerService;
            _navigate = navigate;
            _editingTrainer = trainer;

            Debug.WriteLine("=== AddTrainerViewModel Constructor ===");
            Debug.WriteLine($"Editing mode: {_editingTrainer != null}");

            Directions = new ObservableCollection<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("yoga", "Йога"),
                new KeyValuePair<string, string>("fitness", "Фитнес"),
                new KeyValuePair<string, string>("climbing", "Скалолазание")
            };

            if (_editingTrainer != null)
            {
                LoadTrainerData();
            }
            else
            {
                _selectedDirection = "yoga";
            }
        }

        private void LoadTrainerData()
        {
            if (_editingTrainer == null) return;

            _firstName = _editingTrainer.FirstName;
            _lastName = _editingTrainer.LastName;
            _email = _editingTrainer.Email;
            _description = _editingTrainer.Description;
            _experience = _editingTrainer.Experience;
            _selectedDirection = _editingTrainer.Direction;
            _photo = _editingTrainer.Photo;

            // Кэшируем пароль в отдельном поле для отображения
            // В реальном приложении здесь должно быть "********"
            _password = "********";

            // Обновляем UI
            OnPropertyChanged(nameof(FirstName));
            OnPropertyChanged(nameof(LastName));
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(Experience));
            OnPropertyChanged(nameof(SelectedDirection));
            OnPropertyChanged(nameof(PhotoVisibility));
            OnPropertyChanged(nameof(Password)); // Важно для отображения

            // Уведомляем о возможности сохранения
            OnPropertyChanged(nameof(CanSave));
        }

        public string PageTitle => _editingTrainer == null ? "Добавить тренера" : "Редактировать тренера";
        public string SaveButtonText => _editingTrainer == null ? "Добавить" : "Сохранить";

        public string FirstName
        {
            get => _firstName;
            set { if (SetField(ref _firstName, value)) { ValidateFirstName(); OnPropertyChanged(nameof(CanSave)); } }
        }

        public string LastName
        {
            get => _lastName;
            set { if (SetField(ref _lastName, value)) { ValidateLastName(); OnPropertyChanged(nameof(CanSave)); } }
        }

        public string Email
        {
            get => _email;
            set { if (SetField(ref _email, value)) { ValidateEmail(); OnPropertyChanged(nameof(CanSave)); } }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetField(ref _password, value))
                {
                    // Если в режиме редактирования и пароль не равен "********", значит он был изменен
                    if (_editingTrainer != null && value != "********")
                    {
                        _isPasswordChanged = true;
                    }
                    ValidatePassword();
                    OnPropertyChanged(nameof(CanSave));
                }
            }
        }

        public string Description
        {
            get => _description;
            set { if (SetField(ref _description, value)) OnPropertyChanged(nameof(CanSave)); }
        }

        public int Experience
        {
            get => _experience;
            set { if (SetField(ref _experience, value)) OnPropertyChanged(nameof(CanSave)); }
        }

        public string SelectedDirection
        {
            get => _selectedDirection;
            set { if (SetField(ref _selectedDirection, value)) OnPropertyChanged(nameof(CanSave)); }
        }

        public byte[]? Photo
        {
            get => _photo;
            set
            {
                if (SetField(ref _photo, value))
                {
                    OnPropertyChanged(nameof(PhotoPreview));
                    OnPropertyChanged(nameof(PhotoVisibility));
                }
            }
        }

        public string FirstNameError { get => _firstNameError; private set => SetField(ref _firstNameError, value); }
        public string LastNameError { get => _lastNameError; private set => SetField(ref _lastNameError, value); }
        public string EmailError { get => _emailError; private set => SetField(ref _emailError, value); }
        public string PasswordError { get => _passwordError; private set => SetField(ref _passwordError, value); }

        public Visibility PhotoVisibility => _photo == null ? Visibility.Visible : Visibility.Collapsed;

        public string? PhotoPreview => _photo != null ? "preview" : null;

        public ObservableCollection<KeyValuePair<string, string>> Directions { get; }

        public bool CanSave
        {
            get
            {
                bool baseValidation = !string.IsNullOrWhiteSpace(FirstName) &&
                                      !string.IsNullOrWhiteSpace(LastName) &&
                                      IsValidEmail(Email) &&
                                      !string.IsNullOrWhiteSpace(SelectedDirection);

                if (_editingTrainer == null)
                {
                    // При создании пароль обязателен
                    return baseValidation && !string.IsNullOrWhiteSpace(Password);
                }
                else
                {
                    // При редактировании пароль может быть пустым (тогда он не меняется)
                    return baseValidation;
                }
            }
        }

        public ICommand SaveCommand => new RelayCommand(async () =>
        {
            try
            {
                await ExecuteSave();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SaveCommand error: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }, () => CanSave);

        public ICommand CancelCommand => new RelayCommand(ExecuteCancel);

        private void ValidateFirstName()
        {
            FirstNameError = string.IsNullOrWhiteSpace(FirstName) ? "Обязательное поле" : string.Empty;
        }

        private void ValidateLastName()
        {
            LastNameError = string.IsNullOrWhiteSpace(LastName) ? "Обязательное поле" : string.Empty;
        }

        private void ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                EmailError = "Обязательное поле";
            }
            else if (!IsValidEmail(Email))
            {
                EmailError = "Некорректный формат email";
            }
            else
            {
                EmailError = string.Empty;
            }
        }

        private void ValidatePassword()
        {
            if (_editingTrainer == null)
            {
                // При создании
                if (string.IsNullOrWhiteSpace(Password))
                {
                    PasswordError = "Пароль обязателен";
                }
                else if (Password.Length < 6)
                {
                    PasswordError = "Минимум 6 символов";
                }
                else
                {
                    PasswordError = string.Empty;
                }
            }
            else
            {
                // При редактировании
                if (!string.IsNullOrWhiteSpace(Password) && Password != "********")
                {
                    if (Password.Length < 6)
                    {
                        PasswordError = "Минимум 6 символов";
                    }
                    else
                    {
                        PasswordError = string.Empty;
                    }
                }
                else
                {
                    PasswordError = string.Empty;
                }
            }
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

        private async Task ExecuteSave()
        {
            Debug.WriteLine("=== ExecuteSave ===");

            ValidateFirstName();
            ValidateLastName();
            ValidateEmail();
            ValidatePassword();

            if (!CanSave)
            {
                Debug.WriteLine("Validation failed, cannot save");
                return;
            }

            try
            {
                var trainer = _editingTrainer ?? new Trainer();
                trainer.FirstName = FirstName;
                trainer.LastName = LastName;
                trainer.Email = Email;
                trainer.Description = Description;
                trainer.Experience = Experience;
                trainer.Direction = SelectedDirection;
                trainer.Photo = Photo;

                Debug.WriteLine($"Trainer data prepared:");
                Debug.WriteLine($"  Id: {trainer.Id}");
                Debug.WriteLine($"  FirstName: {trainer.FirstName}");
                Debug.WriteLine($"  LastName: {trainer.LastName}");
                Debug.WriteLine($"  Direction: {trainer.Direction}");
                Debug.WriteLine($"  Email: {trainer.Email}");

                // Обработка пароля
                if (_editingTrainer == null)
                {
                    // При создании - используем введенный пароль
                    trainer.PlainPassword = Password;
                    Debug.WriteLine($"  Password will be set (length: {Password.Length})");
                }
                else
                {
                    // При редактировании
                    if (_isPasswordChanged && !string.IsNullOrWhiteSpace(Password) && Password != "********")
                    {
                        // Пароль был изменен
                        trainer.PlainPassword = Password;
                        Debug.WriteLine($"  Password will be updated (length: {Password.Length})");
                    }
                    else
                    {
                        // Пароль не меняется
                        trainer.PlainPassword = string.Empty;
                        Debug.WriteLine($"  Password will NOT be updated");
                    }
                }

                bool success;
                string successMessage;

                if (_editingTrainer == null)
                {
                    success = await _trainerService.CreateAsync(trainer);
                    successMessage = "Тренер успешно добавлен!";
                }
                else
                {
                    trainer.Id = _editingTrainer.Id;
                    success = await _trainerService.UpdateAsync(trainer);
                    successMessage = "Тренер успешно обновлен!";
                }

                Debug.WriteLine($"Save result: {success}");

                if (success)
                {
                    Debug.WriteLine("Navigate back to TrainersView");
                    MessageBox.Show(successMessage, "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    _navigate(new Views.Trainers.TrainersView(_navigate));
                }
                else
                {
                    Debug.WriteLine("Save failed, showing message");
                    MessageBox.Show("Ошибка сохранения данных.\n" +
                                   "Возможно, тренер с таким email уже существует.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ExecuteSave Exception: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteCancel()
        {
            _navigate(new Views.Trainers.TrainersView(_navigate));
        }
    }
}