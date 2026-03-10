using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Clients;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rockstar.Admin.WPF.Views.Clients
{
    public partial class AddClientView : Page
    {
        private readonly AddClientViewModel _viewModel;

        public AddClientView(Action<Page> navigate, Models.Client? client)
        {
            InitializeComponent();

            var clientService = App.Services.GetRequiredService<IClientService>();
            _viewModel = new AddClientViewModel(clientService, navigate, client);

            DataContext = _viewModel;

            // Подписываемся на изменение пароля
            PasswordBox.PasswordChanged += (s, e) =>
            {
                if (_viewModel != null)
                    _viewModel.Password = PasswordBox.Password;
            };

            // Если редактирование - показываем заглушку
            if (client != null && !string.IsNullOrEmpty(client.PasswordHash))
            {
                PasswordBox.Password = "********";
            }
        }

        private void PhotoBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files|*.jpg;*.jpeg;*.png;*.gif|All files|*.*",
                Title = "Выберите фотографию"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var bytes = File.ReadAllBytes(openFileDialog.FileName);
                    _viewModel.Photo = bytes;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
                _viewModel.Password = ((PasswordBox)sender).Password;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}