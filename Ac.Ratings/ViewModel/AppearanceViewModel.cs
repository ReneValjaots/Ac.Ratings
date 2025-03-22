using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using Ac.Ratings.Core;
using Ac.Ratings.Services;
using Ac.Ratings.Theme.ModernUI.Helpers;

namespace Ac.Ratings.ViewModel;

public class AppearanceViewModel : Core.ViewModel {

    private readonly Color[] _wpAccentColors = new Color[] {
        Color.FromRgb(0xa4, 0xc4, 0x00), // lime
        Color.FromRgb(0x60, 0xa9, 0x17), // green
        Color.FromRgb(0x00, 0x8a, 0x00), // emerald
        Color.FromRgb(0x00, 0xab, 0xa9), // teal
        Color.FromRgb(0x1b, 0xa1, 0xe2), // cyan
        Color.FromRgb(0x00, 0x50, 0xef), // cobalt
        Color.FromRgb(0x6a, 0x00, 0xff), // indigo
        Color.FromRgb(0xaa, 0x00, 0xff), // violet
        Color.FromRgb(0xf4, 0x72, 0xd0), // pink
        Color.FromRgb(0xd8, 0x00, 0x73), // magenta
        Color.FromRgb(0xa2, 0x00, 0x25), // crimson
        Color.FromRgb(0xe5, 0x14, 0x00), // red
        Color.FromRgb(0xfa, 0x68, 0x00), // orange
        Color.FromRgb(0xf0, 0xa3, 0x0a), // amber
        Color.FromRgb(0xe3, 0xc8, 0x00), // yellow
        Color.FromRgb(0x82, 0x5a, 0x2c), // brown
        Color.FromRgb(0x6d, 0x87, 0x64), // olive
        Color.FromRgb(0x64, 0x76, 0x87), // steel
        Color.FromRgb(0x76, 0x60, 0x8a), // mauve
        Color.FromRgb(0x87, 0x79, 0x4e), // taupe
    };

    private Color _selectedAccentColor;
    private ObservableCollection<Link> _themes = new ObservableCollection<Link>();
    private Link _selectedTheme;

    public AppearanceViewModel() {
        _themes.Add(new Link { DisplayName = "black", Source = AppearanceManager.BlackThemeSource });
        _themes.Add(new Link { DisplayName = "dark", Source = AppearanceManager.DarkThemeSource });
        _themes.Add(new Link { DisplayName = "light", Source = AppearanceManager.LightThemeSource });

        LoadAppearanceSettings();
        SyncThemeAndColor();

        // Subscribe to AppearanceManager changes
        PropertyChangedEventManager.AddHandler(AppearanceManager.Current, OnAppearanceManagerPropertyChanged, nameof(AppearanceManager.AccentColor));
        PropertyChangedEventManager.AddHandler(AppearanceManager.Current, OnAppearanceManagerPropertyChanged, nameof(AppearanceManager.ThemeSource));
    }

    private void LoadAppearanceSettings() {
        var savedTheme = ConfigManager.LoadAppearanceConfigValue("Theme");
        _selectedTheme = _themes.FirstOrDefault(t => t.DisplayName == savedTheme) ?? _themes.First();

        var savedColor = ConfigManager.LoadAppearanceConfigValue("AccentColor");
        if (!string.IsNullOrEmpty(savedColor)) {
            try {
                _selectedAccentColor = (Color)ColorConverter.ConvertFromString(savedColor);
            }
            catch (Exception) {
                _selectedAccentColor = _wpAccentColors.First();
            }
        }
        else {
            _selectedAccentColor = _wpAccentColors.First();
        }

        AppearanceManager.Current.ThemeSource = _selectedTheme.Source;
        AppearanceManager.Current.AccentColor = _selectedAccentColor;
    }

    private void SyncThemeAndColor() {
        _selectedTheme = _themes.FirstOrDefault(l => l.Source.Equals(AppearanceManager.Current.ThemeSource)) ?? _themes.First();
        _selectedAccentColor = AppearanceManager.Current.AccentColor != default ? AppearanceManager.Current.AccentColor : _wpAccentColors.First();
        OnPropertyChanged(nameof(SelectedTheme));
        OnPropertyChanged(nameof(SelectedAccentColor));
    }

    private void OnAppearanceManagerPropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(AppearanceManager.AccentColor) or nameof(AppearanceManager.ThemeSource)) {
            SyncThemeAndColor();
        }
    }

    public ObservableCollection<Link> Themes => _themes;

    public Color[] AccentColors => _wpAccentColors;

    public Link SelectedTheme {
        get => _selectedTheme;
        set {
            if (SetField(ref _selectedTheme, value)) {
                AppearanceManager.Current.ThemeSource = value.Source;
                ConfigManager.SaveAppearanceConfigValue("Theme", value.DisplayName);
            }
        }
    }

    public Color SelectedAccentColor {
        get => _selectedAccentColor;
        set {
            if (SetField(ref _selectedAccentColor, value)) {
                AppearanceManager.Current.AccentColor = value;
                ConfigManager.SaveAppearanceConfigValue("AccentColor", value.ToString());
            }
        }
    }
}
