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
            InitializeComponent();

            Action<Page> navigate = (page) => MainFrame.Navigate(page);
            MainFrame.Navigate(new LoginPage(navigate));
        }
    }
}