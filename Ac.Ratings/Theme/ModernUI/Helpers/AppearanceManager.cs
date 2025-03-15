using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Ac.Ratings.Core;

namespace Ac.Ratings.Theme.ModernUI.Helpers {
    /// <summary>
    /// Manages the theme, font size and accent colors for a Modern UI application.
    /// </summary>
    public class AppearanceManager : ObservableObject {
        public static readonly Uri DarkThemeSource = new Uri("Theme/ModernUI/Assets/ModernUI.Dark.xaml", UriKind.Relative);
        public static readonly Uri LightThemeSource = new Uri("Theme/ModernUI/Assets/ModernUI.Light.xaml", UriKind.Relative);
        public static readonly Uri BlackThemeSource = new Uri("Theme/ModernUI/Assets/ModernUI.Black.xaml", UriKind.Relative);

        public const string KeyAccentColor = "AccentColor";
        public const string KeyAccent = "Accent";

        private AppearanceManager() {
            DarkThemeCommand = new RelayCommand(SetDarkTheme, CanSetDarkTheme);
            LightThemeCommand = new RelayCommand(SetLightTheme, CanSetLightTheme);
            BlackThemeCommand = new RelayCommand(SetBlackTheme, CanSetBlackTheme);

            AccentColorCommand = new RelayCommand<object>(SetAccentColorFromObject, CanSetAccentColorFromObject);
        }

        private void SetDarkTheme() => ThemeSource = DarkThemeSource;
        private bool CanSetDarkTheme() => !DarkThemeSource.Equals(ThemeSource);

        private void SetLightTheme() => ThemeSource = LightThemeSource;
        private bool CanSetLightTheme() => !LightThemeSource.Equals(ThemeSource);

        private void SetBlackTheme() => ThemeSource = BlackThemeSource;
        private bool CanSetBlackTheme() => !BlackThemeSource.Equals(ThemeSource);

        private void SetAccentColorFromObject(object obj) {
            if (obj is Color accentColor) {
                AccentColor = accentColor;
            }
            else if (obj is string str) {
                AccentColor = (Color)ColorConverter.ConvertFromString(str);
            }
        }

        private bool CanSetAccentColorFromObject(object obj) {
            return obj is Color or string;
        }


        private ResourceDictionary GetThemeDictionary() {
            // determine the current theme by looking at the app resources and return the first dictionary having the resource key 'WindowBackground' defined.
            return (from dict in Application.Current.Resources.MergedDictionaries
                where dict.Contains("WindowBackground")
                select dict).FirstOrDefault();
        }

        private Uri GetThemeSource() {
            var dict = GetThemeDictionary();
            if (dict != null) {
                return dict.Source;
            }

            // could not determine the theme dictionary
            return null;
        }

        private void SetThemeSource(Uri source, bool useThemeAccentColor) {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var oldThemeDict = GetThemeDictionary();
            var dictionaries = Application.Current.Resources.MergedDictionaries;
            var themeDict = new ResourceDictionary { Source = source };

            // if theme defines an accent color, use it
            if (themeDict[KeyAccentColor] is Color accentColor) {
                // remove from the theme dictionary and apply globally if useThemeAccentColor is true
                themeDict.Remove(KeyAccentColor);

                if (useThemeAccentColor) {
                    ApplyAccentColor(accentColor);
                }
            }

            // add new before removing old theme to avoid dynamicresource not found warnings
            dictionaries.Add(themeDict);

            // remove old theme
            if (oldThemeDict != null) {
                dictionaries.Remove(oldThemeDict);
            }

            OnPropertyChanged(nameof(ThemeSource));
        }

        private void ApplyAccentColor(Color accentColor) {
            Application.Current.Resources[KeyAccentColor] = accentColor;
            Application.Current.Resources[KeyAccent] = new SolidColorBrush(accentColor);
        }

        private Color GetAccentColor() {
            if (Application.Current.Resources[KeyAccentColor] is Color accentColor) {
                return accentColor;
            }

            // default color: teal
            return Color.FromArgb(0xff, 0x1b, 0xa1, 0xe2);
        }

        private void SetAccentColor(Color value) {
            ApplyAccentColor(value);

            // re-apply theme to ensure brushes referencing AccentColor are updated
            var themeSource = GetThemeSource();
            if (themeSource != null) {
                SetThemeSource(themeSource, false);
            }

            OnPropertyChanged(nameof(AccentColor));
        }

        public static AppearanceManager Current { get; } = new AppearanceManager();

        public ICommand DarkThemeCommand { get; private set; }
        public ICommand LightThemeCommand { get; private set; }
        public ICommand BlackThemeCommand { get; private set; }
        public ICommand AccentColorCommand { get; private set; }

        public Uri ThemeSource {
            get => GetThemeSource();
            set => SetThemeSource(value, true);
        }

        public Color AccentColor {
            get => GetAccentColor();
            set => SetAccentColor(value);
        }
    }
}
