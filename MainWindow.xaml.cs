using Rockstar.Admin.WPF.Views.Auth;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Rockstar.Admin.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();

                // 🔑 Логирование для отладки
                System.Diagnostics.Debug.WriteLine("=== MainWindow Constructor ===");
                System.Diagnostics.Debug.WriteLine($"MainFrame initialized: {MainFrame != null}");

                // Создание делегата навигации
                Action<Page> navigate = (page) =>
                {
                    System.Diagnostics.Debug.WriteLine($"Navigating to: {page.GetType().Name}");
                    MainFrame.Navigate(page);
                };

                // Переход на страницу авторизации
                System.Diagnostics.Debug.WriteLine("Creating LoginPage...");
                var loginPage = new LoginPage(navigate);
                System.Diagnostics.Debug.WriteLine("Navigating to LoginPage...");
                MainFrame.Navigate(loginPage);
                System.Diagnostics.Debug.WriteLine("=== MainWindow Constructor Complete ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MainWindow Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);

                MessageBox.Show(
                    $"Ошибка в MainWindow:\n\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}