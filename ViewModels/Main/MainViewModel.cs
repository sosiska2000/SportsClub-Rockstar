using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Base;
using Rockstar.Admin.WPF.Views.Auth;
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

        // 🔑 Оставить только ОДНО определение каждой команды
        public ICommand OpenClientsCommand => new RelayCommand(() => _navigate(new Views.Clients.ClientsView(_navigate)));
        public ICommand OpenTrainersCommand => new RelayCommand(() => _navigate(new Views.Trainers.TrainersView(_navigate)));
        public ICommand OpenSubscriptionsCommand => new RelayCommand(() => _navigate(new Views.Subscriptions.SubscriptionsView(_navigate)));
        public ICommand OpenDirectionsCommand => new RelayCommand(() => _navigate(new Views.Directions.DirectionsView(_navigate)));
        public ICommand OpenScheduleCommand => new RelayCommand(() => _navigate(new Views.Schedule.ScheduleView(_navigate)));
        public ICommand LogoutCommand => new RelayCommand(async () => await ExecuteLogout());

        private async Task ExecuteLogout()
        {
            await _authService.LogoutAsync();
            _navigate(new LoginPage(_navigate));
        }
    }
}