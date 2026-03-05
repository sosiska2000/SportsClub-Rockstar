using System;
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

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // Сервисы
            services.AddSingleton<IAuthService, MockAuthService>();
            services.AddSingleton<ITrainerService, MockTrainerService>();

            // Окна
            services.AddTransient<MainWindow>();
        }
    }
}