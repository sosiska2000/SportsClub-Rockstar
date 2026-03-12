using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rockstar.Admin.WPF.ViewModels.Subscriptions
{
    public class SubscriptionsViewModel : ViewModelBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IDirectionService _directionService;
        private readonly Action<Page> _navigate;

        private ObservableCollection<Subscription> _subscriptions = new();
        private ObservableCollection<Subscription> _allSubscriptions = new();
        private ObservableCollection<Direction> _directions = new();
        private Direction? _selectedDirectionFilter;
        private string _searchText = string.Empty;

        public SubscriptionsViewModel(
            ISubscriptionService subscriptionService,
            IDirectionService directionService,
            Action<Page> navigate)
        {
            _subscriptionService = subscriptionService;
            _directionService = directionService;
            _navigate = navigate;

            LoadDataAsync();
        }

        public ObservableCollection<Subscription> Subscriptions
        {
            get => _subscriptions;
            private set => SetField(ref _subscriptions, value);
        }

        public ObservableCollection<Direction> Directions
        {
            get => _directions;
            private set => SetField(ref _directions, value);
        }

        public Direction? SelectedDirectionFilter
        {
            get => _selectedDirectionFilter;
            set
            {
                if (SetField(ref _selectedDirectionFilter, value))
                {
                    ApplyFilters();
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetField(ref _searchText, value))
                {
                    ApplyFilters();
                }
            }
        }

        public ICommand AddSubscriptionCommand => new RelayCommand(ExecuteAddSubscription);
        public ICommand EditSubscriptionCommand => new RelayCommand<Subscription>(ExecuteEditSubscription);
        public ICommand DeleteSubscriptionCommand => new RelayCommand<Subscription>(async s => await ExecuteDeleteSubscription(s));
        public ICommand ClearFiltersCommand => new RelayCommand(ExecuteClearFilters);

        private async void LoadDataAsync()
        {
            try
            {
                Debug.WriteLine("=== LoadDataAsync started ===");

                var subscriptions = await _subscriptionService.GetAllAsync();
                Debug.WriteLine($"Loaded {subscriptions.Count} subscriptions");

                var directions = await _directionService.GetAllAsync();
                Debug.WriteLine($"Loaded {directions.Count} directions");

                _allSubscriptions = new ObservableCollection<Subscription>(subscriptions);
                Directions = new ObservableCollection<Direction>(directions);

                ApplyFilters();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadDataAsync error: {ex.Message}");
            }
        }

        private void ApplyFilters()
        {
            try
            {
                var filtered = _allSubscriptions.AsEnumerable();

                // Фильтр по направлению
                if (_selectedDirectionFilter != null)
                {
                    filtered = filtered.Where(s => s.DirectionId == _selectedDirectionFilter.Id);
                }

                // Поиск по названию
                if (!string.IsNullOrWhiteSpace(_searchText))
                {
                    var searchLower = _searchText.ToLower();
                    filtered = filtered.Where(s =>
                        s.Name.ToLower().Contains(searchLower) ||
                        (s.Description?.ToLower().Contains(searchLower) ?? false));
                }

                Subscriptions = new ObservableCollection<Subscription>(filtered);
                Debug.WriteLine($"Filtered to {Subscriptions.Count} subscriptions");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ApplyFilters error: {ex.Message}");
            }
        }

        private void ExecuteAddSubscription()
        {
            _navigate(new Views.Subscriptions.AddSubscriptionView(_navigate, null));
        }

        private void ExecuteEditSubscription(Subscription subscription)
        {
            if (subscription != null)
            {
                Debug.WriteLine($"Editing subscription ID: {subscription.Id}");
                _navigate(new Views.Subscriptions.AddSubscriptionView(_navigate, subscription));
            }
        }

        private async Task ExecuteDeleteSubscription(Subscription subscription)
        {
            if (subscription == null) return;

            var result = System.Windows.MessageBox.Show(
                $"Удалить абонемент '{subscription.Name}'?",
                "Подтверждение удаления",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    Debug.WriteLine($"Deleting subscription ID: {subscription.Id}");
                    var success = await _subscriptionService.DeleteAsync(subscription.Id);

                    if (success)
                    {
                        Debug.WriteLine("Subscription deleted successfully");
                        _allSubscriptions.Remove(subscription);
                        ApplyFilters();

                        System.Windows.MessageBox.Show("Абонемент успешно удален!", "Успех",
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    }
                    else
                    {
                        Debug.WriteLine("Failed to delete subscription");
                        System.Windows.MessageBox.Show("Не удалось удалить абонемент.", "Ошибка",
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Delete error: {ex.Message}");
                    System.Windows.MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private void ExecuteClearFilters()
        {
            SelectedDirectionFilter = null;
            SearchText = string.Empty;
        }
    }
}