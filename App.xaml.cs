using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Rockstar.Admin.WPF.Services;
using Rockstar.Admin.WPF.Services.Interfaces;

namespace Rockstar.Admin.WPF
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            Services = services.BuildServiceProvider();

            // Запуск MainWindow
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // 🔑 Регистрируем ТОЛЬКО сервисы, не Views и не ViewModels

            // Сервис авторизации (Mock для тестов)
            services.AddSingleton<IAuthService, MockAuthService>();

            // Если будет реальный API:
            // services.AddHttpClient<IAuthService, AuthService>(client =>
            // {
            //     client.BaseAddress = new Uri("https://localhost:5001/api/");
            // });
        }
    }
}