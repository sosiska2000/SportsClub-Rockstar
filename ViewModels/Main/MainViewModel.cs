using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Base;
using Rockstar.Admin.WPF.Views.Auth;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rockstar.Admin.WPF.ViewModels.Main
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly Action<Page> _navigate;

        public MainViewModel(IAuthService authService, Action<Page> navigate)
        {
            _authService = authService;
            _navigate = navigate;
        }

        public ICommand OpenClientsCommand => new RelayCommand(_ => NavigateToSection("Clients"));
        public ICommand OpenSubscriptionsCommand => new RelayCommand(_ => NavigateToSection("Subscriptions"));
        public ICommand OpenTrainersCommand => new RelayCommand(_ => NavigateToSection("Trainers"));
        public ICommand OpenDirectionsCommand => new RelayCommand(_ => NavigateToSection("Directions"));
        public ICommand OpenScheduleCommand => new RelayCommand(_ => NavigateToSection("Schedule"));
        public ICommand LogoutCommand => new RelayCommand(async _ => await ExecuteLogout());

        private void NavigateToSection(string section)
        {
            System.Diagnostics.Debug.WriteLine($"Navigate to: {section}");
        }

        private async Task ExecuteLogout()
        {
            await _authService.LogoutAsync();
            _navigate(new LoginPage(_navigate));
        }
    }
}