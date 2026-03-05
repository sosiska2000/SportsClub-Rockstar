using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Base;
using System;
using System.Threading.Tasks;
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

        // 🔑 Все команды без параметра
        public ICommand OpenClientsCommand => new RelayCommand(() => NavigateToSection("Clients"));
        public ICommand OpenSubscriptionsCommand => new RelayCommand(() => NavigateToSection("Subscriptions"));
        public ICommand OpenTrainersCommand => new RelayCommand(() => NavigateToSection("Trainers"));
        public ICommand OpenDirectionsCommand => new RelayCommand(() => NavigateToSection("Directions"));
        public ICommand OpenScheduleCommand => new RelayCommand(() => NavigateToSection("Schedule"));
        public ICommand LogoutCommand => new RelayCommand(async () => await ExecuteLogout());

        private void NavigateToSection(string section)
        {
            System.Diagnostics.Debug.WriteLine($"Navigate to: {section}");
        }

        private async Task ExecuteLogout()
        {
            await _authService.LogoutAsync();
            _navigate(new Views.Auth.LoginPage(_navigate));
        }
    }
}