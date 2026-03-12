using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Commands;
using Rockstar.Admin.WPF.Views.Clients;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rockstar.Admin.WPF.ViewModels.Clients
{
    public class ClientsViewModel : INotifyPropertyChanged
    {
        private readonly IClientService _clientService;
        private readonly Action<Page> _navigate;

        private ObservableCollection<Client> _clients = new();
        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set
            {
                _clients = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadClientsCommand { get; }
        public ICommand AddClientCommand { get; }
        public ICommand EditClientCommand { get; }
        public ICommand DeleteClientCommand { get; }
        public ICommand DebugCommand { get; }

        public ClientsViewModel(IClientService clientService, Action<Page> navigate)
        {
            _clientService = clientService;
            _navigate = navigate;

            LoadClientsCommand = new AsyncRelayCommand(async () => await LoadClients());
            AddClientCommand = new RelayCommand(() => AddClient());
            EditClientCommand = new AsyncRelayCommand<Client>(async (client) => await EditClient(client));
            DeleteClientCommand = new AsyncRelayCommand<Client>(async (client) => await DeleteClient(client));
            DebugCommand = new AsyncRelayCommand(async () => await DebugDelete());

            // Загружаем данные при создании
            LoadClientsCommand.Execute(null);
        }

        private async Task LoadClients()
        {
            try
            {
                Debug.WriteLine("Loading clients...");
                var clients = await _clientService.GetAllAsync();
                Debug.WriteLine($"Loaded {clients.Count} clients");

                Clients = new ObservableCollection<Client>(clients);
                Debug.WriteLine("Clients collection updated");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadClients error: {ex.Message}");
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddClient()
        {
            _navigate(new AddClientView(_navigate, null));
        }

        private async Task EditClient(Client? client)
        {
            if (client == null) return;
            _navigate(new AddClientView(_navigate, client));
        }

        private async Task DeleteClient(Client? client)
        {
            if (client == null) return;

            Debug.WriteLine($"Attempting to delete client: {client.Id} - {client.FullName}");

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить клиента {client.FullName}?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var success = await _clientService.DeleteAsync(client.Id);
                    Debug.WriteLine($"Delete result: {success}");

                    if (success)
                    {
                        // Просто удаляем из коллекции на клиенте
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Clients.Remove(client);
                        });

                        Debug.WriteLine($"Client removed from collection, remaining: {Clients.Count}");

                        MessageBox.Show("Клиент успешно удален!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        Debug.WriteLine("Delete failed - no rows affected");
                        MessageBox.Show("Ошибка при удалении клиента!", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Delete exception: {ex.Message}");
                    MessageBox.Show($"Ошибка при удалении клиента: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task DebugDelete()
        {
            try
            {
                Debug.WriteLine("=== DEBUG DELETE START ===");

                var allClients = await _clientService.GetAllIncludingDeletedAsync();
                Debug.WriteLine($"=== ALL CLIENTS (including deleted) - {allClients.Count} total ===");

                foreach (var client in allClients)
                {
                    Debug.WriteLine($"ID: {client.Id}, Name: {client.FullName}, Email: {client.Email}, Active: {client.IsActive}");
                }

                var activeClient = allClients.FirstOrDefault(c => c.IsActive);
                if (activeClient != null)
                {
                    Debug.WriteLine($"\nTesting delete on active client ID: {activeClient.Id}, Name: {activeClient.FullName}");

                    var result = await _clientService.DeleteAsync(activeClient.Id);
                    Debug.WriteLine($"Delete result: {result}");

                    var afterDelete = await _clientService.GetAllIncludingDeletedAsync();
                    var deleted = afterDelete.FirstOrDefault(c => c.Id == activeClient.Id);
                    if (deleted != null)
                    {
                        Debug.WriteLine($"Client after delete: ID={deleted.Id}, IsActive={deleted.IsActive}");
                    }

                    var activeClients = await _clientService.GetAllAsync();
                    Debug.WriteLine($"Active clients count after delete: {activeClients.Count}");
                }
                else
                {
                    Debug.WriteLine("No active clients found for testing");
                }

                await LoadClients();

                Debug.WriteLine("=== DEBUG DELETE END ===");

                MessageBox.Show("Отладка завершена. Проверьте Output window для результатов.",
                    "Отладка", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Debug error: {ex.Message}");
                Debug.WriteLine($"Debug stack trace: {ex.StackTrace}");
                MessageBox.Show($"Ошибка отладки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}