using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using Ac.Ratings.Core;
using Ac.Ratings.Model;
using Ac.Ratings.Services;

namespace Ac.Ratings.ViewModel {
    public class SettingsViewModel : Core.ViewModel {
        private readonly IDialogService _dialogService;
        private string _selectedPrimaryUnit;
        private string _selectedSecondaryUnit;
        private ObservableCollection<Car> _carDb;

        public RelayCommand ResetRatingsCommand { get; }
        public RelayCommand ResetExtraFeaturesCommand { get; }
        public RelayCommand RestoreBackupCommand { get; }
        public RelayCommand SaveSettingsCommand { get; }
        public RelayCommand ResetRootFolderCommand { get; }
        public RelayCommand TransferRatingsCommand { get; }


        public SettingsViewModel(IDialogService dialogService) {
            _dialogService = dialogService;

            ResetRatingsCommand = new RelayCommand(ResetAllRatings);
            ResetExtraFeaturesCommand = new RelayCommand(ResetAllExtraFeatures);
            RestoreBackupCommand = new RelayCommand(RestoreBackup);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            ResetRootFolderCommand = new RelayCommand(ResetRootFolder);
            TransferRatingsCommand = new RelayCommand(TransferRatings);

            LoadSettings(ConfigManager.ConfigFilePath);
        }
        public string SelectedPrimaryUnit {
            get => _selectedPrimaryUnit;
            set => SetField(ref _selectedPrimaryUnit, value);
        }

        public string SelectedSecondaryUnit {
            get => _selectedSecondaryUnit;
            set => SetField(ref _selectedSecondaryUnit, value);
        }

        public void SetCarDb(ObservableCollection<Car> carDb) {
            _carDb = carDb;
        }

        public void LoadSettings(string configPath) {
            if (File.Exists(configPath)) {
                var json = File.ReadAllText(configPath);
                var config = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (config != null) {
                    SelectedPrimaryUnit = config.GetValueOrDefault("PrimaryPowerUnit", "kw").ToLower();
                    SelectedSecondaryUnit = config.GetValueOrDefault("SecondaryPowerUnit", "hp").ToLower();
                }
            }
            else {
                SelectedPrimaryUnit = "kw";
                SelectedSecondaryUnit = "hp";
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

            File.WriteAllText(configPath, JsonSerializer.Serialize(config, ConfigManager.JsonOptions));
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
                SaveSettings(ConfigManager.ConfigFilePath);
                _dialogService.ShowMessage("Settings saved successfully.", "Success", MessageBoxButton.OK);
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
            if (_carDb == null) {
                _dialogService.ShowMessage("Car database is not initialized.", "Error", MessageBoxButton.OK);
                return;
            }

            if (_dialogService.ShowConfirmation("Are you sure you want to reset all ratings? This action cannot be undone.", "Confirm Reset")) {
                CarDataManager.ResetAllRatingsInDatabase(_carDb);
                _dialogService.ShowMessage("All ratings have been reset successfully.", "Success", MessageBoxButton.OK);
            }
            else {
                _dialogService.ShowMessage("Reset operation canceled.", "Cancel", MessageBoxButton.OK);
            }
        }

        private void ResetAllExtraFeatures() {
            if (_carDb == null) {
                _dialogService.ShowMessage("Car database is not initialized.", "Error", MessageBoxButton.OK);
                return;
            }

            if (_dialogService.ShowConfirmation("Are you sure you want to reset all extra features? This action cannot be undone.", "Confirm Reset")) {
                CarDataManager.ResetAllExtraFeaturesInDatabase(_carDb);
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

            if (_carDb == null) {
                _dialogService.ShowMessage("Car database is not initialized.", "Error", MessageBoxButton.OK);
                return;
            }

            try {
                var restoredCarDb = CarDataManager.RestoreCarDbFromBackup(backupFilePath);
                if (restoredCarDb != null) {
                    _carDb.Clear();
                    foreach (var car in restoredCarDb) {
                        _carDb.Add(car);
                        CarDataManager.SaveCarToFile(car);
                    }

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
