using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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

        public AddTrainerViewModel(ITrainerService trainerService, Action<Page> navigate, Trainer? trainer)
        {
            _trainerService = trainerService;
            _navigate = navigate;
            _editingTrainer = trainer;

            Directions = new ObservableCollection<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("yoga", "Йога"),
                new KeyValuePair<string, string>("fitness", "Фитнес"),
                new KeyValuePair<string, string>("climbing", "Скалолазание")
            };

            if (_editingTrainer != null)
            {
                _firstName = _editingTrainer.FirstName;
                _lastName = _editingTrainer.LastName;
                _email = _editingTrainer.Email;
                _description = _editingTrainer.Description;
                _experience = _editingTrainer.Experience;
                _selectedDirection = _editingTrainer.Direction;
                _photo = _editingTrainer.Photo;
                OnPropertyChanged(nameof(PhotoPreview));
                OnPropertyChanged(nameof(PhotoVisibility));
            }
            else
            {
                _selectedDirection = "yoga";
            }
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
            set { if (SetField(ref _password, value)) { ValidatePassword(); OnPropertyChanged(nameof(CanSave)); } }
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
        public string? PhotoPreview => _photo != null ? "data:image" : null;

        public ObservableCollection<KeyValuePair<string, string>> Directions { get; }

        public bool CanSave => !string.IsNullOrWhiteSpace(FirstName) &&
                               !string.IsNullOrWhiteSpace(LastName) &&
                               IsValidEmail(Email) &&
                               (_editingTrainer != null || !string.IsNullOrWhiteSpace(Password)) &&
                               !string.IsNullOrWhiteSpace(SelectedDirection);

        // 🔑 ИСПРАВЛЕНИЕ: используем лямбды без параметров для не-универсального RelayCommand
        public ICommand SaveCommand => new RelayCommand(ExecuteSaveAsync, () => CanSave);
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
            if (_editingTrainer == null && string.IsNullOrWhiteSpace(Password))
            {
                PasswordError = "Пароль обязателен";
            }
            else if (!string.IsNullOrWhiteSpace(Password) && Password.Length < 6)
            {
                PasswordError = "Минимум 6 символов";
            }
            else
            {
                PasswordError = string.Empty;
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

        // 🔑 Отдельный метод для async-логики, вызываемый из RelayCommand
        private async void ExecuteSaveAsync()
        {
            await ExecuteSave();
        }

        private async Task ExecuteSave()
        {
            ValidateFirstName();
            ValidateLastName();
            ValidateEmail();
            ValidatePassword();

            if (!CanSave) return;

            var trainer = _editingTrainer ?? new Trainer();
            trainer.FirstName = FirstName;
            trainer.LastName = LastName;
            trainer.Email = Email;
            trainer.Description = Description;
            trainer.Experience = Experience;
            trainer.Direction = SelectedDirection;
            trainer.Photo = Photo;

            if (!string.IsNullOrWhiteSpace(Password))
            {
                trainer.PasswordHash = Password;
            }

            bool success = _editingTrainer == null
                ? await _trainerService.CreateAsync(trainer)
                : await _trainerService.UpdateAsync(trainer);

            if (success)
            {
                _navigate(new Views.Trainers.TrainersView(_navigate));
            }
            else
            {
                MessageBox.Show("Ошибка сохранения данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteCancel()
        {
            _navigate(new Views.Trainers.TrainersView(_navigate));
        }
    }
}