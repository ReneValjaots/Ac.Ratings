using Ac.Ratings.Services;
using Ac.Ratings.Theme.ModernUI.Helpers;
using System.Windows;
using System.Windows.Media;
using Ac.Ratings.Core;
using Ac.Ratings.View;
using Ac.Ratings.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Ac.Ratings.Services.Interfaces;

namespace Ac.Ratings {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private readonly IServiceProvider _serviceProvider;
        public SettingsViewModel SettingsViewModel => _serviceProvider.GetRequiredService<SettingsViewModel>();
        public AppearanceViewModel AppearanceViewModel => _serviceProvider.GetRequiredService<AppearanceViewModel>();


        public App() {
            IServiceCollection services = new ServiceCollection();
            services.ConfigureServices();
            _serviceProvider = services.BuildServiceProvider();
        }


        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            
            _ = ConfigManager.ResourceFolder;

            var acRootFolderWindowFactory = _serviceProvider.GetRequiredService<Func<AcRootFolderWindow>>();

            Func<string?> promptAction = () => {
                var acRootFolderWindow = acRootFolderWindowFactory();

                if (acRootFolderWindow.ShowDialog() == true) {
                    return acRootFolderWindow.SelectedPath;
                }

                return null; // User cancelled
            };

            bool isAcRootConfigured = ConfigManager.EnsureAcRootFolderConfigured(promptAction);

            if (!isAcRootConfigured) {
                MessageBox.Show("Assetto Corsa root folder is required but was not configured correctly. The application will now exit.", "Configuration Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Environment.Exit(1); 
                return; 
            }

            ApplyAppearanceSettings();
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
   
        }


        private void ApplyAppearanceSettings() {
            var savedTheme = ConfigManager.LoadAppearanceConfigValue("Theme");
            if (!string.IsNullOrEmpty(savedTheme)) {
                AppearanceManager.Current.ThemeSource = savedTheme.ToLower() switch {
                    "dark" => AppearanceManager.DarkThemeSource,
                    "black" => AppearanceManager.BlackThemeSource,
                    "light" => AppearanceManager.LightThemeSource,
                    _ => AppearanceManager.DarkThemeSource
                };
            }
            else {
                AppearanceManager.Current.ThemeSource = AppearanceManager.DarkThemeSource;
            }

            var savedColor = ConfigManager.LoadAppearanceConfigValue("AccentColor");
            if (!string.IsNullOrEmpty(savedColor)) {
                try {
                    var color = (Color)ColorConverter.ConvertFromString(savedColor);
                    AppearanceManager.Current.AccentColor = color;
                }
                catch (Exception) {
                    AppearanceManager.Current.AccentColor = Color.FromRgb(0x1b, 0xa1, 0xe2);
                }
            }
            else {
                AppearanceManager.Current.AccentColor = Color.FromRgb(0x1b, 0xa1, 0xe2);
            }
        }
    }

    public static class ServiceCollectionExtensions {
        public static void ConfigureServices(this IServiceCollection services) {
            services.AddSingleton<ICarDataService, CarDataService>();
            services.AddSingleton<ICarDisplayService, CarDisplayService>();

            services.AddSingleton<IDialogService>(sp => {
                var mainWindow = sp.GetRequiredService<MainWindow>();
                return new ModernDialogService(mainWindow);
            });

            services.AddSingleton<MainViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<AppearanceViewModel>();
            services.AddTransient<AcRootFolderViewModel>();

            services.AddSingleton<MainWindow>();

            services.AddSingleton<Func<Type, Core.ViewModel>>(serviceProvider => viewModelType =>
                (Core.ViewModel)serviceProvider.GetRequiredService(viewModelType));

            services.AddTransient<Func<AcRootFolderWindow>>(sp => () => {
                var viewModel = sp.GetRequiredService<AcRootFolderViewModel>();
                var window = new AcRootFolderWindow(viewModel);
                var dialogService = new ModernDialogService(window);
                viewModel.SetDialogService(dialogService);
                return window;
            });
        }
    }
}
