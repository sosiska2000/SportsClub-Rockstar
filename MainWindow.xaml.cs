using System;
using System.Windows;
using System.Windows.Controls;
using Rockstar.Admin.WPF.Views.Auth;

namespace Rockstar.Admin.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Создаём делегат навигации
            Action<Page> navigate = (page) => MainFrame.Navigate(page);

            // Создаём LoginPage вручную, передавая делегат
            var loginPage = new LoginPage(navigate);
            MainFrame.Navigate(loginPage);
        }
    }
}