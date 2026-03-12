using Microsoft.Extensions.DependencyInjection;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Clients;
using Rockstar.Admin.WPF.Views.Main;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Rockstar.Admin.WPF.Views.Clients
{
    public partial class ClientsView : Page
    {
        private readonly Action<Page> _navigate;
        private readonly ClientsViewModel _viewModel;

        public ClientsView(Action<Page> navigate)
        {
            try
            {
                Debug.WriteLine("ClientsView: Initializing...");

                InitializeComponent();

                _navigate = navigate;

                var clientService = App.Services.GetRequiredService<IClientService>();
                Debug.WriteLine("ClientsView: ClientService obtained");

                _viewModel = new ClientsViewModel(clientService, navigate);
                Debug.WriteLine("ClientsView: ViewModel created");

                DataContext = _viewModel;
                Debug.WriteLine("ClientsView: DataContext set");

                // Подписываемся на событие загрузки страницы
                this.Loaded += (s, e) =>
                {
                    Debug.WriteLine("ClientsView: Loaded event fired");
                    _viewModel.LoadClientsCommand.Execute(null);
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ClientsView constructor error: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                MessageBox.Show($"Ошибка инициализации страницы: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _navigate(new MainPage(_navigate));
        }
    }
}