using Ac.Ratings.Services;
using Ac.Ratings.Theme.ModernUI.Helpers;
using System.Windows;
using System.Windows.Media;
using Ac.Ratings.Core;
using Ac.Ratings.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Ac.Ratings.Data;

namespace Ac.Ratings {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private readonly ServiceProvider _serviceProvider;

        public App() {
            IServiceCollection services = new ServiceCollection();
            services.ConfigureServices();
            _serviceProvider = services.BuildServiceProvider();
        }


        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            
            _ = ConfigManager.ResourceFolder;

            Func<string?> promptAction = () => {
                var acRootFolderWindow = new AcRootFolderWindow();
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
                    _ => AppearanceManager.BlackThemeSource
                };
            }
            else {
                AppearanceManager.Current.ThemeSource = AppearanceManager.BlackThemeSource;
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
            services.AddSingleton<MainWindow>(provider => new MainWindow {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });

            services.AddSingleton<MainViewModel>();
            services.AddTransient<SettingsViewModel>();

            services.AddSingleton<Func<Type, Core.ViewModel>>(serviceProvider => viewModelType =>
                (Core.ViewModel)serviceProvider.GetRequiredService(viewModelType));
        }
    }
}
