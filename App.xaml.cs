using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rockstar.Admin.WPF.Services;
using Rockstar.Admin.WPF.Services.Database;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.Views.Auth;

namespace Rockstar.Admin.WPF
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;
        public static IConfiguration Configuration { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Загрузка конфигурации
                Configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                var services = new ServiceCollection();
                ConfigureServices(services);
                Services = services.BuildServiceProvider();

                // 🔑 Создание и показ главного окна
                var mainWindow = new MainWindow();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                // 🔑 Ловим все ошибки и показываем их
                MessageBox.Show(
                    $"Ошибка при запуске приложения:\n\n{ex.Message}\n\n{ex.StackTrace}",
                    "Критическая ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                // Завершаем приложение
                Shutdown();
            }
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // Регистрация IConfiguration
            services.AddSingleton<IConfiguration>(Configuration);

            // База данных
            services.AddSingleton<MySqlDbContext>();

            // Сервисы
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<ITrainerService, TrainerService>();

            // Тестовый сервис
            services.AddSingleton<DatabaseTestService>();

            // Логи
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Debug);
            });
        }
    }
}