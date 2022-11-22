using System;
using System.Windows;
using Djambi.Engine.Services;
using Djambi.UI.Pages;
using Djambi.UI.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace Djambi.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IThemeLoader, ThemeLoader>();
            services.AddSingleton<IImageRepository, ImageRepository>();
            services.AddSingleton<IBrushRepository, BrushRepository>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<GamePage>();
            services.AddSingleton<MainMenuPage>();
            services.AddSingleton<ValidationService>();
            services.AddSingleton<PageFactory>(pageName =>
            {
                switch (pageName) {
                    case nameof(MainMenuPage): return _serviceProvider.GetService<MainMenuPage>();
                    case nameof(GamePage): return _serviceProvider.GetService<GamePage>();
                    default: throw new Exception($"Invalid page name: {pageName}");
                }
            });
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _serviceProvider.GetService<IThemeLoader>().LoadTheme("Default");
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
