using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services;
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
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;

namespace Rockstar.Admin.WPF
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;
        public static IConfiguration Configuration { get; private set; } = null!;
        public static bool UseApi { get; set; } = true; // Всегда используем API

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

                // Создаем LoginPage с правильным делегатом
                var loginPage = new LoginPage(Services, page =>
                {
                    _navigationService.NavigateTo(page);
                });

                _navigationService.NavigateTo(loginPage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}\n\nStackTrace: {ex.StackTrace}",
                    "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

            // Регистрируем HttpClient
            services.AddHttpClient();

            // Регистрируем ApiService
            services.AddSingleton<IApiService>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var baseUrl = config["ApiSettings:BaseUrl"] ?? "http://localhost:5143/api/";
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri(baseUrl);
                httpClient.Timeout = TimeSpan.FromSeconds(30);

                return new ApiService(httpClient);
            });

            // 👇 ВАЖНО: Используем ТОЛЬКО ApiAuthService, НЕ MockAuthService!
            services.AddSingleton<IAuthService, ApiAuthService>();

            // API сервисы
            services.AddSingleton<IClientService, ApiClientService>();
            services.AddSingleton<ITrainerService, ApiTrainerService>();
            services.AddSingleton<IScheduleService, ApiScheduleService>();
            services.AddSingleton<ISubscriptionService, ApiSubscriptionService>();
            services.AddSingleton<IDirectionService, ApiDirectionService>();
            services.AddSingleton<IServiceService, ApiServiceService>();

            // Временная заглушка для IServiceTypeService (если не используется - можно удалить)
            services.AddSingleton<IServiceTypeService, MockServiceTypeService>();

            // ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<ClientsViewModel>();
            services.AddTransient<TrainersViewModel>();
            services.AddTransient<ScheduleViewModel>();
            services.AddTransient<SubscriptionsViewModel>();
            services.AddTransient<DirectionsViewModel>();
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

    // Временная заглушка для ServiceTypeService
    public class MockServiceTypeService : IServiceTypeService
    {
        public Task<List<ServiceType>> GetAllAsync() => Task.FromResult(new List<ServiceType>());
        public Task<List<ServiceType>> GetByDirectionIdAsync(int directionId) => Task.FromResult(new List<ServiceType>());
        public Task<ServiceType?> GetByIdAsync(int id) => Task.FromResult<ServiceType?>(null);
        public Task<bool> CreateAsync(ServiceType serviceType) => Task.FromResult(true);
        public Task<bool> UpdateAsync(ServiceType serviceType) => Task.FromResult(true);
        public Task<bool> DeleteAsync(int id) => Task.FromResult(true);
    }
}