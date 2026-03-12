using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Trainers;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Rockstar.Admin.WPF.Views.Trainers
{
    public partial class AddTrainerView : Page
    {
        private readonly AddTrainerViewModel _viewModel;

        public AddTrainerView(Action<Page> navigate, Models.Trainer? trainer)
        {
            InitializeComponent();

            var trainerService = App.Services.GetRequiredService<ITrainerService>();
            _viewModel = new AddTrainerViewModel(trainerService, navigate, trainer);

            DataContext = _viewModel;

            // Привязка пароля
            PasswordBox.PasswordChanged += (s, e) =>
            {
                _viewModel.Password = PasswordBox.Password;
            };

            // Если редактирование — заполняем пароль визуально
            if (trainer != null && !string.IsNullOrEmpty(trainer.PasswordHash))
            {
                PasswordBox.Password = "********";
            }

            // Если есть фото при редактировании, отображаем его
            if (trainer?.Photo != null && trainer.Photo.Length > 0)
            {
                LoadPhotoFromBytes(trainer.Photo);
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
                    LoadPhotoFromBytes(bytes);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadPhotoFromBytes(byte[] imageBytes)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(imageBytes);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                PhotoImage.Source = bitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading photo: {ex.Message}");
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = PasswordBox.Password;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}