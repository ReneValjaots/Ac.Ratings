using Ac.Ratings.Services;
using Ac.Ratings.Theme.ModernUI.Helpers;
using System.Windows;
using System.Windows.Media;

namespace Ac.Ratings {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            ApplyAppearanceSettings();
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
                // Default to black if no saved theme
                AppearanceManager.Current.ThemeSource = AppearanceManager.BlackThemeSource;
            }

            // Load accent color
            var savedColor = ConfigManager.LoadAppearanceConfigValue("AccentColor");
            if (!string.IsNullOrEmpty(savedColor)) {
                try {
                    var color = (Color)ColorConverter.ConvertFromString(savedColor);
                    AppearanceManager.Current.AccentColor = color;
                }
                catch (Exception) {
                    // Fallback to default accent color if parsing fails
                    AppearanceManager.Current.AccentColor = Color.FromRgb(0x1b, 0xa1, 0xe2);
                }
            }
            else {
                AppearanceManager.Current.AccentColor = Color.FromRgb(0x1b, 0xa1, 0xe2);
            }
        }
    }
}
