using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rockstar.Admin.WPF.ViewModels.Directions
{
    public class DirectionDetailViewModel : ViewModelBase
    {
        private readonly IDirectionService _directionService;
        private readonly IServiceService _serviceService;
        private readonly IServiceTypeService _serviceTypeService;
        private readonly Action<Page> _navigate;
        private readonly string _directionKey;

        private Direction? _direction;
        private ObservableCollection<Service> _services = new();
        private ObservableCollection<ServiceType> _serviceTypes = new();

        public DirectionDetailViewModel(
            IDirectionService directionService,
            IServiceService serviceService,
            IServiceTypeService serviceTypeService,
            Action<Page> navigate,
            string directionKey)
        {
            _directionService = directionService;
            _serviceService = serviceService;
            _serviceTypeService = serviceTypeService;
            _navigate = navigate;
            _directionKey = directionKey;

            LoadDataAsync();
        }

        public Direction? Direction
        {
            get => _direction;
            private set => SetField(ref _direction, value);
        }

        public ObservableCollection<Service> Services
        {
            get => _services;
            private set => SetField(ref _services, value);
        }

        public ObservableCollection<ServiceType> ServiceTypes
        {
            get => _serviceTypes;
            private set => SetField(ref _serviceTypes, value);
        }

        public string PageTitle => Direction?.Name ?? "Направление";

        public ICommand AddServiceCommand => new RelayCommand(ExecuteAddService);
        public ICommand EditServiceCommand => new RelayCommand<Service>(ExecuteEditService);
        public ICommand DeleteServiceCommand => new RelayCommand<Service>(async s => await ExecuteDeleteService(s));
        public ICommand AddServiceTypeCommand => new RelayCommand(ExecuteAddServiceType);
        public ICommand EditServiceTypeCommand => new RelayCommand<ServiceType>(ExecuteEditServiceType);
        public ICommand DeleteServiceTypeCommand => new RelayCommand<ServiceType>(async st => await ExecuteDeleteServiceType(st));
        public ICommand BackCommand => new RelayCommand(ExecuteBack);

        private async void LoadDataAsync()
        {
            try
            {
                Debug.WriteLine($"=== LoadDataAsync for {_directionKey} ===");

                Direction = await _directionService.GetByKeyAsync(_directionKey);

                if (Direction == null)
                {
                    Debug.WriteLine($"Direction not found for key: {_directionKey}");
                    return;
                }

                var services = await _serviceService.GetByDirectionIdAsync(Direction.Id);
                Services = new ObservableCollection<Service>(services);

                var serviceTypes = await _serviceTypeService.GetByDirectionIdAsync(Direction.Id);
                ServiceTypes = new ObservableCollection<ServiceType>(serviceTypes);

                OnPropertyChanged(nameof(PageTitle));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadDataAsync error: {ex.Message}");
            }
        }

        private void ExecuteAddService()
        {
            MessageBox.Show("Функция добавления услуги будет реализована позже", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExecuteEditService(Service service)
        {
            if (service == null) return;

            MessageBox.Show($"Редактирование услуги: {service.Name}", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task ExecuteDeleteService(Service service)
        {
            if (service == null) return;

            var result = MessageBox.Show(
                $"Удалить услугу '{service.Name}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var success = await _serviceService.DeleteAsync(service.Id);
                    if (success)
                    {
                        Services.Remove(service);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Delete error: {ex.Message}");
                }
            }
        }

        private void ExecuteAddServiceType()
        {
            MessageBox.Show("Функция добавления типа услуги будет реализована позже", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExecuteEditServiceType(ServiceType serviceType)
        {
            if (serviceType == null) return;

            MessageBox.Show($"Редактирование типа: {serviceType.Name}", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task ExecuteDeleteServiceType(ServiceType serviceType)
        {
            if (serviceType == null) return;

            var result = MessageBox.Show(
                $"Удалить тип услуги '{serviceType.Name}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var success = await _serviceTypeService.DeleteAsync(serviceType.Id);
                    if (success)
                    {
                        ServiceTypes.Remove(serviceType);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Delete error: {ex.Message}");
                }
            }
        }

        private void ExecuteBack()
        {
            _navigate(new Views.Directions.DirectionsView(_navigate));
        }
    }
}