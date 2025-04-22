using System.IO;
using System.Text.Json;
using System.Windows;
using Ac.Ratings.Core;
using Ac.Ratings.Services;
using Ac.Ratings.Services.Interfaces;

namespace Ac.Ratings.ViewModel {
    public class SettingsViewModel : Core.ViewModel {
        private readonly IDialogService _dialogService;
        private readonly ICarDataService _carDataService;
        private RatingRoundingMode _selectedRoundingMode;
        private string _selectedPrimaryUnit;
        private string _selectedSecondaryUnit;
        private int _selectedRatingScale;

        public RelayCommand ResetRatingsCommand { get; }
        public RelayCommand ResetExtraFeaturesCommand { get; }
        public RelayCommand RestoreBackupCommand { get; }
        public RelayCommand SaveSettingsCommand { get; }
        public RelayCommand ResetRootFolderCommand { get; }
        public RelayCommand TransferRatingsCommand { get; }

        public SettingsViewModel(IDialogService dialogService, ICarDataService carDataService) {
            _dialogService = dialogService;
            _carDataService = carDataService ?? throw new ArgumentNullException(nameof(carDataService));

            ResetRatingsCommand = new RelayCommand(ResetAllRatings);
            ResetExtraFeaturesCommand = new RelayCommand(ResetAllExtraFeatures);
            RestoreBackupCommand = new RelayCommand(RestoreBackup);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            ResetRootFolderCommand = new RelayCommand(ResetRootFolder);
            TransferRatingsCommand = new RelayCommand(TransferRatings);

            LoadSettings(ConfigManager.ConfigFilePath);
            SelectedRatingScale = ConfigManager.RatingScaleMaximum;
            SelectedRoundingMode = ConfigManager.RatingRounding;
        }

        public string SelectedPrimaryUnit {
            get => _selectedPrimaryUnit;
            set => SetField(ref _selectedPrimaryUnit, value);
        }

        public string SelectedSecondaryUnit {
            get => _selectedSecondaryUnit;
            set => SetField(ref _selectedSecondaryUnit, value);
        }

        public int SelectedRatingScale {
            get => _selectedRatingScale;
            set => SetField(ref _selectedRatingScale, value);
        }

        public RatingRoundingMode SelectedRoundingMode {
            get => _selectedRoundingMode;
            set => SetField(ref _selectedRoundingMode, value);
        }

        public IEnumerable<int> AvailableRatingScales => new List<int> { 5, 10 };
        public IEnumerable<RatingRoundingMode> RoundingModes => Enum.GetValues<RatingRoundingMode>();

        public void LoadSettings(string configPath) {
            if (File.Exists(configPath)) {
                var json = File.ReadAllText(configPath);
                var config = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (config != null) {
                    SelectedPrimaryUnit = config.GetValueOrDefault("PrimaryPowerUnit", "kw").ToLower();
                    SelectedSecondaryUnit = config.GetValueOrDefault("SecondaryPowerUnit", "hp").ToLower();
                    SelectedRatingScale = ConfigManager.RatingScaleMaximum;
                    SelectedRoundingMode = ConfigManager.RatingRounding;
                }
            }
            else {
                SelectedPrimaryUnit = "kw";
                SelectedSecondaryUnit = "hp";
                SelectedRatingScale = 10;
                SelectedRoundingMode = RatingRoundingMode.RoundDown;
            }
        }

        public void SaveSettings(string configPath) {
            if (!File.Exists(configPath)) {
                throw new FileNotFoundException("Config file not found. Cannot save settings without a valid config file.", configPath);
            }

            var json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            if (config == null) {
                throw new InvalidOperationException(
                    "The configuration file could not be parsed. " +
                    "It must be in a valid dictionary format, where each setting consists of a name and a value. " +
                    "For example: \"PrimaryPowerUnit\": \"ps\". Both the name and the value must be enclosed in double quotes and separated by a colon (:).");
            }

            config["PrimaryPowerUnit"] = SelectedPrimaryUnit.ToLower();
            config["SecondaryPowerUnit"] = SelectedSecondaryUnit.ToLower();
            config["RatingScaleMaximum"] = SelectedRatingScale.ToString();
            config["RatingRoundingMode"] = SelectedRoundingMode.ToString();

            File.WriteAllText(configPath, JsonSerializer.Serialize(config, ConfigManager.JsonOptions));

            ConfigManager.SaveRatingScaleMaximum(SelectedRatingScale);
            ConfigManager.SaveRebaseRoundingMode(SelectedRoundingMode);
        }

        private void TransferRatings() {
            try {
                var decoder = new RatingsDecoder();
                decoder.InitializeRatingsDataFile();
                decoder.InitializeUserRatings();
                decoder.ExportDataFile();
                _dialogService.ShowMessage("Ratings exported successfully!", "Success", MessageBoxButton.OK);
            }
            catch (Exception ex) {
                _dialogService.ShowMessage($"An error occurred during export: {ex.Message}", "Error", MessageBoxButton.OK);
            }
        }

        private void SaveSettings() {
            try {
                int previousScale = ConfigManager.RatingScaleMaximum;
                SaveSettings(ConfigManager.ConfigFilePath);
                int newScale = ConfigManager.RatingScaleMaximum;

                if (previousScale != newScale) {
                    try {
                        _carDataService.RecalculateAllRatingsScale(previousScale, newScale, SelectedRoundingMode);
                        _dialogService.ShowMessage($"Settings saved and ratings recalculated successfully. The application will now close to finalize the changes.", "Success", MessageBoxButton.OK);
                        Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
                    }
                    catch (Exception ex) {
                        ErrorLogger.LogError("RecalculationTrigger", ex);
                        _dialogService.ShowMessage($"Settings saved, but an error occurred during rating recalculation: {ex.Message}\nCheck logs for details.", "Recalculation Error", MessageBoxButton.OK);
                    }
                }
                else {
                    _dialogService.ShowMessage("Settings saved successfully.", "Success", MessageBoxButton.OK);
                }
            }
            catch (FileNotFoundException ex) {
                _dialogService.ShowMessage($"Config file not found: {ex.Message}", "Error", MessageBoxButton.OK);
            }
            catch (InvalidOperationException ex) {
                _dialogService.ShowMessage($"Invalid config format: {ex.Message}", "Error", MessageBoxButton.OK);
            }
            catch (Exception ex) {
                _dialogService.ShowMessage($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK);
            }
        }

        private void ResetAllRatings() {
            if (_dialogService.ShowConfirmation("Are you sure you want to reset all ratings? This action cannot be undone.", "Confirm Reset")) {
                _carDataService.ResetAllRatings();
                _dialogService.ShowMessage("All ratings have been reset successfully.", "Success", MessageBoxButton.OK);
            }
            else {
                _dialogService.ShowMessage("Reset operation canceled.", "Cancel", MessageBoxButton.OK);
            }
        }

        private void ResetAllExtraFeatures() {
            if (_dialogService.ShowConfirmation("Are you sure you want to reset all extra features? This action cannot be undone.", "Confirm Reset")) {
                _carDataService.ResetAllExtraFeatures();
                _dialogService.ShowMessage("All extra features have been reset successfully.", "Success", MessageBoxButton.OK);
            }
            else {
                _dialogService.ShowMessage("Reset operation canceled.", "Cancel", MessageBoxButton.OK);
            }
        }

        private void RestoreBackup() {
            string backupFilePath = _dialogService.ShowOpenFileDialog("Select Backup File", "JSON Files (*.json)|*.json", ConfigManager.BackupFolder);
            if (string.IsNullOrEmpty(backupFilePath)) {
                _dialogService.ShowMessage("No backup file selected.", "Error", MessageBoxButton.OK);
                return;
            }

            try {
                _carDataService.RestoreCarDbFromBackup(backupFilePath);
                if (_carDataService.CarDb.Any()) {
                    _dialogService.ShowMessage("Car database restored successfully.", "Success", MessageBoxButton.OK);
                }
            }
            catch (Exception ex) {
                _dialogService.ShowMessage($"Failed to restore CarDb from backup: {ex.Message}", "Error", MessageBoxButton.OK);
            }
        }

        private void ResetRootFolder() {
            if (_dialogService.ShowConfirmation("This will reset the root folder and close the application. Are you sure?", "Confirm Reset")) {
                try {
                    if (File.Exists(ConfigManager.ConfigFilePath)) {
                        File.Delete(ConfigManager.ConfigFilePath);
                        Environment.Exit(0);
                    }
                    else {
                        _dialogService.ShowMessage("No configuration file was found to reset.", "Error", MessageBoxButton.OK);
                    }
                }
                catch (Exception ex) {
                    _dialogService.ShowMessage($"An error occurred while resetting: {ex.Message}", "Error", MessageBoxButton.OK);
                }
            }
            else {
                _dialogService.ShowMessage("Reset operation canceled.", "Cancel", MessageBoxButton.OK);
            }
        }
    }
}
