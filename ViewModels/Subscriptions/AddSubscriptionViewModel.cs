using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Base;
using Rockstar.Admin.WPF.ViewModels.Commands;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rockstar.Admin.WPF.ViewModels.Subscriptions
{
    public class AddSubscriptionViewModel : ViewModelBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IDirectionService _directionService;
        private readonly Action<Page> _navigate;
        private readonly Subscription? _editingSubscription;

        private string _name = string.Empty;
        private Direction? _selectedDirection;
        private decimal _price;
        private int _sessionsCount = 1;
        private string _description = string.Empty;
        private string _nameError = string.Empty;
        private string _priceError = string.Empty;
        private string _sessionsError = string.Empty;

        public AddSubscriptionViewModel(
            ISubscriptionService subscriptionService,
            IDirectionService directionService,
            Action<Page> navigate,
            Subscription? subscription)
        {
            _subscriptionService = subscriptionService;
            _directionService = directionService;
            _navigate = navigate;
            _editingSubscription = subscription;

            Debug.WriteLine("=== AddSubscriptionViewModel Constructor ===");
            Debug.WriteLine($"Editing mode: {_editingSubscription != null}");

            LoadDirectionsAsync();

            if (_editingSubscription != null)
            {
                LoadSubscriptionData();
            }
        }

        public string PageTitle => _editingSubscription == null ? "Добавить абонемент" : "Редактировать абонемент";
        public string SaveButtonText => _editingSubscription == null ? "Добавить" : "Сохранить";

        public string Name
        {
            get => _name;
            set { if (SetField(ref _name, value)) { ValidateName(); OnPropertyChanged(nameof(CanSave)); } }
        }

        public Direction? SelectedDirection
        {
            get => _selectedDirection;
            set { if (SetField(ref _selectedDirection, value)) OnPropertyChanged(nameof(CanSave)); }
        }

        public decimal Price
        {
            get => _price;
            set { if (SetField(ref _price, value)) { ValidatePrice(); OnPropertyChanged(nameof(CanSave)); } }
        }

        public int SessionsCount
        {
            get => _sessionsCount;
            set { if (SetField(ref _sessionsCount, value)) { ValidateSessions(); OnPropertyChanged(nameof(CanSave)); } }
        }

        public string Description
        {
            get => _description;
            set { if (SetField(ref _description, value)) OnPropertyChanged(nameof(CanSave)); }
        }

        public string NameError
        {
            get => _nameError;
            private set => SetField(ref _nameError, value);
        }

        public string PriceError
        {
            get => _priceError;
            private set => SetField(ref _priceError, value);
        }

        public string SessionsError
        {
            get => _sessionsError;
            private set => SetField(ref _sessionsError, value);
        }

        public ObservableCollection<Direction> Directions { get; } = new();

        public bool CanSave =>
            !string.IsNullOrWhiteSpace(Name) &&
            SelectedDirection != null &&
            Price > 0 &&
            SessionsCount > 0;

        public ICommand SaveCommand => new RelayCommand(async () => await ExecuteSave(), () => CanSave);
        public ICommand CancelCommand => new RelayCommand(ExecuteCancel);

        private async void LoadDirectionsAsync()
        {
            try
            {
                var directions = await _directionService.GetAllAsync();
                Directions.Clear();
                foreach (var dir in directions)
                {
                    Directions.Add(dir);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadDirectionsAsync error: {ex.Message}");
            }
        }

        private void LoadSubscriptionData()
        {
            if (_editingSubscription == null) return;

            _name = _editingSubscription.Name;
            _price = _editingSubscription.Price;
            _sessionsCount = _editingSubscription.SessionsCount;
            _description = _editingSubscription.Description;

            // Находим направление по ID
            if (_editingSubscription.DirectionId.HasValue)
            {
                _selectedDirection = Directions.FirstOrDefault(d => d.Id == _editingSubscription.DirectionId.Value);
            }

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Price));
            OnPropertyChanged(nameof(SessionsCount));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(SelectedDirection));
            OnPropertyChanged(nameof(CanSave));
        }

        private void ValidateName()
        {
            NameError = string.IsNullOrWhiteSpace(Name) ? "Введите название абонемента" : string.Empty;
        }

        private void ValidatePrice()
        {
            PriceError = Price <= 0 ? "Стоимость должна быть больше 0" : string.Empty;
        }

        private void ValidateSessions()
        {
            SessionsError = SessionsCount <= 0 ? "Количество занятий должно быть больше 0" : string.Empty;
        }

        private async Task ExecuteSave()
        {
            Debug.WriteLine("=== ExecuteSave ===");

            ValidateName();
            ValidatePrice();
            ValidateSessions();

            if (!CanSave)
            {
                Debug.WriteLine("Validation failed, cannot save");
                return;
            }

            try
            {
                var subscription = _editingSubscription ?? new Subscription();
                subscription.Name = Name;
                subscription.DirectionId = SelectedDirection?.Id;
                subscription.Price = Price;
                subscription.SessionsCount = SessionsCount;
                subscription.Description = Description;

                Debug.WriteLine($"Subscription data prepared:");
                Debug.WriteLine($"  Name: {subscription.Name}");
                Debug.WriteLine($"  DirectionId: {subscription.DirectionId}");
                Debug.WriteLine($"  Price: {subscription.Price}");
                Debug.WriteLine($"  SessionsCount: {subscription.SessionsCount}");

                bool success;
                string successMessage;

                if (_editingSubscription == null)
                {
                    success = await _subscriptionService.CreateAsync(subscription);
                    successMessage = "Абонемент успешно добавлен!";
                }
                else
                {
                    subscription.Id = _editingSubscription.Id;
                    success = await _subscriptionService.UpdateAsync(subscription);
                    successMessage = "Абонемент успешно обновлен!";
                }

                Debug.WriteLine($"Save result: {success}");

                if (success)
                {
                    Debug.WriteLine("Navigate back to SubscriptionsView");
                    MessageBox.Show(successMessage, "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    _navigate(new Views.Subscriptions.SubscriptionsView(_navigate));
                }
                else
                {
                    Debug.WriteLine("Save failed, showing message");
                    MessageBox.Show("Ошибка сохранения данных.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ExecuteSave Exception: {ex.Message}");
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteCancel()
        {
            _navigate(new Views.Subscriptions.SubscriptionsView(_navigate));
        }
    }
}