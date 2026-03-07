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

        public ICommand AddTrainerCommand => new RelayCommand(ExecuteAddTrainer);
        public ICommand ResetFilterCommand => new RelayCommand(ExecuteResetFilter);
        public ICommand EditTrainerCommand => new RelayCommand<Trainer>(ExecuteEditTrainer);
        public ICommand DeleteTrainerCommand => new RelayCommand<Trainer>(async t => await ExecuteDeleteTrainer(t));

        private async void LoadTrainersAsync()
        {
            try
            {
                Debug.WriteLine("=== LoadTrainersAsync started ===");
                var trainers = await _trainerService.GetAllAsync();
                Debug.WriteLine($"Loaded {trainers.Count} trainers from service");

                foreach (var trainer in trainers)
                {
                    Debug.WriteLine($"Trainer: ID={trainer.Id}, {trainer.FirstName} {trainer.LastName}, Direction: {trainer.Direction}");
                }

                _allTrainers = new ObservableCollection<Trainer>(trainers);
                ApplyFilter();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadTrainersAsync error: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                System.Windows.MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                    "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            Debug.WriteLine($"ApplyFilter: SelectedDirection = '{_selectedDirection}'");

            if (string.IsNullOrEmpty(_selectedDirection))
            {
                Trainers = new ObservableCollection<Trainer>(_allTrainers);
                Debug.WriteLine($"Showing all {_allTrainers.Count} trainers");
            }
            else
            {
                var filtered = _allTrainers.Where(t => t.Direction == _selectedDirection).ToList();
                Trainers = new ObservableCollection<Trainer>(filtered);
                Debug.WriteLine($"Filtered to {filtered.Count} trainers with direction '{_selectedDirection}'");
            }
        }

        private void ExecuteAddTrainer()
        {
            _navigate(new Views.Trainers.AddTrainerView(_navigate, null));
        }

        private void ExecuteEditTrainer(Trainer trainer)
        {
            if (trainer != null)
            {
                Debug.WriteLine($"Editing trainer: ID={trainer.Id}, {trainer.FullName}");
                _navigate(new Views.Trainers.AddTrainerView(_navigate, trainer));
            }
        }

        private async Task ExecuteDeleteTrainer(Trainer trainer)
        {
            if (trainer == null) return;

            var result = System.Windows.MessageBox.Show(
                $"Вы действительно хотите удалить тренера {trainer.FullName}?",
                "Подтверждение удаления",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    Debug.WriteLine($"=== ExecuteDeleteTrainer ===");
                    Debug.WriteLine($"Trainer ID: {trainer.Id}");
                    Debug.WriteLine($"Trainer Name: {trainer.FullName}");

                    var success = await _trainerService.DeleteAsync(trainer.Id);

                    Debug.WriteLine($"Delete result from service: {success}");

                    if (success)
                    {
                        Debug.WriteLine("Removing trainer from local collection");
                        _allTrainers.Remove(trainer);
                        ApplyFilter();

                        System.Windows.MessageBox.Show("Тренер успешно удален!", "Успех",
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    }
                    else
                    {
                        Debug.WriteLine("Failed to delete trainer");
                        System.Windows.MessageBox.Show("Не удалось удалить тренера. Проверьте логи.", "Ошибка",
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Delete error: {ex.Message}");
                    Debug.WriteLine(ex.StackTrace);
                    System.Windows.MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
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