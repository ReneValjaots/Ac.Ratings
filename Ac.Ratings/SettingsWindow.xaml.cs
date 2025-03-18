using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Ac.Ratings.Model;
using Ac.Ratings.Services;
using Ac.Ratings.Theme.ModernUI.Controls;
using Ac.Ratings.Theme.ModernUI.Helpers;
using Ac.Ratings.ViewModel;

namespace Ac.Ratings {
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : ModernWindowBase {
        public SettingsViewModel ViewModel { get; } = new();

        public SettingsWindow(ObservableCollection<Car> carDb) {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.SetCarDb(carDb);
            ViewModel.LoadSettings(ConfigManager.ConfigFilePath);
            ViewModel.Notification += ViewModel_Notification;
        }

        public void ViewModel_Notification(object sender, string message) {
            ModernDialog.ShowMessage(message, message.Contains("error") || message.Contains("failed") ? "Error" : "Information",
                message.Contains("error") || message.Contains("failed") ? MessageBoxButton.OK : MessageBoxButton.OK);
        }

        public void OnSaveClick(object sender, RoutedEventArgs e) {
            try {
                ViewModel.SaveSettingsCommand.Execute(null);
                Close();
            }
            catch (FileNotFoundException ex) {
                ModernDialog.ShowMessage(ex.Message, "Config File Missing", MessageBoxButton.OK);
            }
            catch (InvalidOperationException ex) {
                ModernDialog.ShowMessage(ex.Message, "Invalid Config Format", MessageBoxButton.OK);
            }
            catch (Exception ex) {
                ModernDialog.ShowMessage($"An unexpected error occurred while saving power formats: {ex.Message}", "Error", MessageBoxButton.OK);
            }
        }

        public void ResetRatingsButton_Click(object sender, RoutedEventArgs e) {
            var result = ModernDialog.ShowMessage(
                "Are you sure you want to reset all ratings? This action cannot be undone.",
                "Confirm Reset",
                MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes) {
                try {
                    ViewModel.ResetRatingsCommand.Execute(null);
                }
                catch (Exception ex) {
                    ModernDialog.ShowMessage($"Failed to reset ratings: {ex.Message}", "Error", MessageBoxButton.OK);
                }
            }
            else {
                ModernDialog.ShowMessage("Reset operation canceled.", "Cancel", MessageBoxButton.OK);
            }
        }

        public void ResetExtraFeatures_Click(object sender, RoutedEventArgs e) {
            var result = ModernDialog.ShowMessage(
                "Are you sure you want to reset all extra features? This action cannot be undone.",
                "Confirm Reset",
                MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes) {
                try {
                    ViewModel.ResetExtraFeaturesCommand.Execute(null);
                }
                catch (Exception ex) {
                    ModernDialog.ShowMessage($"Failed to reset extra features: {ex.Message}", "Error", MessageBoxButton.OK);
                }
            }
            else {
                ModernDialog.ShowMessage("Reset operation canceled.", "Cancel", MessageBoxButton.OK);
            }
        }

        public void TransferRatingsButton_OnClick(object sender, RoutedEventArgs e) {
            try {
                var decoder = new RatingsDecoder();
                decoder.InitializeRatingsDataFile();
                decoder.InitializeUserRatings();
                decoder.ExportDataFile();
                ModernDialog.ShowMessage("Ratings exported successfully!", "Success", MessageBoxButton.OK);
            }
            catch (Exception ex) {
                ModernDialog.ShowMessage($"An error occurred during export: {ex.Message}", "Error", MessageBoxButton.OK);
            }
        }

        public void RestoreBackupButton_Click(object sender, RoutedEventArgs e) {
            try {
                ViewModel.RestoreBackupCommand.Execute(null);
            }
            catch (Exception ex) {
                ModernDialog.ShowMessage($"Failed to restore backup: {ex.Message}", "Error", MessageBoxButton.OK);
            }
        }

        public void ResetRootFolder_Click(object sender, RoutedEventArgs e) {
            try {
                ViewModel.ResetRootFolderCommand.Execute(null);
            }
            catch (Exception ex) {
                ModernDialog.ShowMessage($"Failed to reset root folder: {ex.Message}", "Error", MessageBoxButton.OK);
            }
        }
    }
}
