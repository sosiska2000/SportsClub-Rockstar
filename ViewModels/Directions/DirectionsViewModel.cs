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

namespace Rockstar.Admin.WPF.ViewModels.Directions
{
    public class DirectionsViewModel : ViewModelBase
    {
        private readonly IDirectionService _directionService;
        private readonly Action<Page> _navigate;

        private ObservableCollection<Direction> _directions = new();

        public DirectionsViewModel(IDirectionService directionService, Action<Page> navigate)
        {
            _directionService = directionService;
            _navigate = navigate;
            LoadDirectionsAsync();
        }

        public ObservableCollection<Direction> Directions
        {
            get => _directions;
            private set => SetField(ref _directions, value);
        }

        public ICommand NavigateToYogaCommand => new RelayCommand(() => ExecuteNavigateToDirection("yoga"));
        public ICommand NavigateToFitnessCommand => new RelayCommand(() => ExecuteNavigateToDirection("fitness"));
        public ICommand NavigateToClimbingCommand => new RelayCommand(() => ExecuteNavigateToDirection("climbing"));

        private async void LoadDirectionsAsync()
        {
            try
            {
                Debug.WriteLine("=== LoadDirectionsAsync started ===");
                var directions = await _directionService.GetAllAsync();
                Debug.WriteLine($"Loaded {directions.Count} directions");

                Directions = new ObservableCollection<Direction>(directions);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadDirectionsAsync error: {ex.Message}");
            }
        }

        private void ExecuteNavigateToDirection(string directionKey)
        {
            _navigate(new Views.Directions.DirectionDetailView(_navigate, directionKey));
        }
    }
}