using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rockstar.Admin.WPF.Services;
using Rockstar.Admin.WPF.Services.Database;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Auth;
using Rockstar.Admin.WPF.ViewModels.Clients;
using Rockstar.Admin.WPF.ViewModels.Directions;
using Rockstar.Admin.WPF.ViewModels.Main;
using Rockstar.Admin.WPF.ViewModels.Schedule;
using Rockstar.Admin.WPF.ViewModels.Subscriptions;
using Rockstar.Admin.WPF.ViewModels.Trainers;
using Rockstar.Admin.WPF.Views.Auth;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Rockstar.Admin.WPF
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;
        public static IConfiguration Configuration { get; private set; } = null!;
        public static bool UseApi { get; set; } = true;

        // Сервис навигации
        private NavigationService _navigationService = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                Configuration = builder.Build();

                var services = new ServiceCollection();
                ConfigureServices(services);
                Services = services.BuildServiceProvider();

                // Получаем сервис навигации
                _navigationService = Services.GetRequiredService<NavigationService>();

                // Открываем главное окно
                var mainWindow = new MainWindow();
                mainWindow.Show();

                // Навигируем на LoginPage (оборачиваем в лямбду!)
                var loginPage = new LoginPage(Services, (page) => _navigationService.NavigateTo(page));
                _navigationService.NavigateTo(loginPage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Критическая ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<NavigationService>();

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            if (UseApi)
            {
                // API режим
                services.AddSingleton<IApiService>(sp =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    var baseUrl = config["Api:BaseUrl"] ?? "https://localhost:7001/api/";
                    return new ApiService(baseUrl);
                });

                services.AddSingleton<IAuthService, ApiAuthService>();
                services.AddSingleton<IClientService, ApiClientService>();
                services.AddSingleton<ITrainerService, ApiTrainerService>();
                services.AddSingleton<IScheduleService, ApiScheduleService>();

                // Временно используем заглушки
                services.AddSingleton<ISubscriptionService, SubscriptionService>();
                services.AddSingleton<IDirectionService, DirectionService>();
                services.AddSingleton<IServiceService, ServiceService>();

                services.AddSingleton<MySqlDbContext>(sp => null!);
            }
            else
            {
                // Прямой доступ к БД
                services.AddSingleton<MySqlDbContext>();
                services.AddSingleton<IAuthService, AuthService>();
                services.AddSingleton<IClientService, ClientService>();
                services.AddSingleton<ITrainerService, TrainerService>();
                services.AddSingleton<IScheduleService, ScheduleService>();
                services.AddSingleton<ISubscriptionService, ApiSubscriptionService>();
                services.AddSingleton<IDirectionService, ApiDirectionService>();
                services.AddSingleton<IServiceService, ApiServiceService>();

                services.AddSingleton<IApiService>(sp => null!);
            }

            // ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<ClientsViewModel>();
            services.AddTransient<TrainersViewModel>();
            services.AddTransient<ScheduleViewModel>();
            services.AddTransient<SubscriptionsViewModel>();
            services.AddTransient<DirectionsViewModel>();

            services.AddSingleton<DatabaseTestService>();
        }
    }

    public class NavigationService
    {
        public event EventHandler<Page>? NavigateRequested;

        public void NavigateTo(Page page)
        {
            NavigateRequested?.Invoke(this, page);
        }
    }
}