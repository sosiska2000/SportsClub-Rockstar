using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Base;

namespace Rockstar.Admin.WPF.ViewModels.Trainers
{
    public class TrainersViewModel : ViewModelBase
    {
        private readonly ITrainerService _trainerService;
        private readonly Action<Page> _navigate;

        private ObservableCollection<Trainer> _trainers = new();
        private ObservableCollection<Trainer> _allTrainers = new();
        private string _selectedDirection = string.Empty;

        public TrainersViewModel(ITrainerService trainerService, Action<Page> navigate)
        {
            _trainerService = trainerService;
            _navigate = navigate;
            LoadTrainersAsync();
        }

        public ObservableCollection<Trainer> Trainers
        {
            get => _trainers;
            private set => SetField(ref _trainers, value);
        }

        public string SelectedDirection
        {
            get => _selectedDirection;
            set
            {
                if (SetField(ref _selectedDirection, value))
                {
                    ApplyFilter();
                }
            }
        }

        public ObservableCollection<KeyValuePair<string, string>> Directions { get; } = new()
        {
            new KeyValuePair<string, string>(string.Empty, "Все направления"),
            new KeyValuePair<string, string>("yoga", "Йога"),
            new KeyValuePair<string, string>("fitness", "Фитнес"),
            new KeyValuePair<string, string>("climbing", "Скалолазание")
        };

        // 🔑 Команды без параметра
        public ICommand AddTrainerCommand => new RelayCommand(ExecuteAddTrainer);
        
        // 🔑 Команды с параметром Trainer?
        public ICommand EditTrainerCommand => new RelayCommand<Trainer?>(ExecuteEditTrainer);
        public ICommand DeleteTrainerCommand => new RelayCommand<Trainer?>(async t => await ExecuteDeleteTrainer(t));
        public ICommand ResetFilterCommand => new RelayCommand(ExecuteResetFilter);

        private async void LoadTrainersAsync()
        {
            var trainers = await _trainerService.GetAllAsync();
            _allTrainers = new ObservableCollection<Trainer>(trainers);
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrEmpty(_selectedDirection))
            {
                Trainers = new ObservableCollection<Trainer>(_allTrainers);
            }
            else
            {
                var filtered = _allTrainers.Where(t => t.Direction == _selectedDirection).ToList();
                Trainers = new ObservableCollection<Trainer>(filtered);
            }
        }

        private void ExecuteAddTrainer()
        {
            _navigate(new Views.Trainers.AddTrainerView(_navigate, null));
        }

        private void ExecuteEditTrainer(Trainer? trainer)
        {
            if (trainer != null)
            {
                _navigate(new Views.Trainers.AddTrainerView(_navigate, trainer));
            }
        }

        private async Task ExecuteDeleteTrainer(Trainer? trainer)
        {
            if (trainer == null) return;

            var result = System.Windows.MessageBox.Show(
                $"Удалить тренера {trainer.FullName}?",
                "Подтверждение",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                var success = await _trainerService.DeleteAsync(trainer.Id);
                if (success)
                {
                    _allTrainers.Remove(trainer);
                    ApplyFilter();
                }
            }
        }

        private void ExecuteResetFilter()
        {
            SelectedDirection = string.Empty;
        }

        public void Refresh()
        {
            LoadTrainersAsync();
        }
    }
}