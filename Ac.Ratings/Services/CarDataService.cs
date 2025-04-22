using Ac.Ratings.Model;
using Ac.Ratings.Services.Interfaces;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Ac.Ratings.Services {
    public class CarDataService : ICarDataService {
        private readonly List<Car> _modifiedCars = new();
        private const int _maxBackupCount = 10;
        public ObservableCollection<Car> CarDb { get; }

        public CarDataService() {
            CarDb = new ObservableCollection<Car>(LoadCarDatabase());
        }

        private List<Car> LoadCarDatabase() {
            if (string.IsNullOrEmpty(ConfigManager.AcRootFolder))
                return new List<Car>();

            var factory = new CarFactory(ConfigManager.AcRootFolder, ConfigManager.CarsRootFolder);
            return factory.InitializeCars();
        }

        public void MarkCarAsModified(Car car) {
            if (!_modifiedCars.Contains(car)) {
                _modifiedCars.Add(car);
            }
        }

        public void SaveModifiedCars() {
            foreach (var car in _modifiedCars) {
                SaveCarToFile(car);
            }

            _modifiedCars.Clear();
        }

        private void SaveCarToFile(Car car) {
            if (string.IsNullOrEmpty(ConfigManager.CarsRootFolder)) {
                throw new ArgumentException("Cars root folder path is null or empty.");
            }

            if (string.IsNullOrEmpty(car.FolderName)) {
                throw new ArgumentException($"Folder name for car {car.Name} is null or empty.");
            }

            var carFolderPath = Path.Combine(ConfigManager.CarsRootFolder, car.FolderName);
            var carJsonFilePath = Path.Combine(carFolderPath, "RatingsApp", "ui.json");
            var jsonContent = JsonSerializer.Serialize(car, ConfigManager.JsonOptions);
            File.WriteAllText(carJsonFilePath, jsonContent);
        }

        public void CreateBackupOfCarDb() {
            string backupFolder = Path.Combine(ConfigManager.BackupFolder, "backups");
            Directory.CreateDirectory(backupFolder);


            string backupFileName = $"CarDb_backup_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.json";
            string backupFilePath = Path.Combine(backupFolder, backupFileName);

            var jsonContent = JsonSerializer.Serialize(CarDb, ConfigManager.JsonOptions);
            File.WriteAllText(backupFilePath, jsonContent);

            var backupFiles = Directory.GetFiles(backupFolder, "CarDb_backup_*.json")
                .OrderByDescending(File.GetCreationTime)
                .ToList();

            if (backupFiles.Count > _maxBackupCount) {
                foreach (var oldBackup in backupFiles.Skip(_maxBackupCount)) {
                    File.Delete(oldBackup);
                }
            }
        }

        public void ResetAllRatings() {
            try {
                CreateBackupOfCarDb();

                foreach (var car in CarDb) {
                    car?.Ratings?.ResetRatingValues();
                    SaveCarToFile(car);
                }
            }
            catch (Exception ex) {
                ErrorLogger.LogError("ResetAllRatings", ex);
            }
        }

        public void ResetAllExtraFeatures() {
            try {
                CreateBackupOfCarDb();

                foreach (var car in CarDb) {
                    car?.Ratings?.ResetExtraFeatureValues();
                    SaveCarToFile(car);
                }
            }
            catch (Exception ex) {
                ErrorLogger.LogError("ResetAllExtraFeatures", ex);
            }
        }

        public void RestoreCarDbFromBackup(string backupFilePath) {
            if (!File.Exists(backupFilePath)) {
                throw new FileNotFoundException("Selected backup file not found.");
            }

            var jsonContent = File.ReadAllText(backupFilePath);
            var restoredCarDb = JsonSerializer.Deserialize<List<Car>>(jsonContent, ConfigManager.JsonOptions);
            if (restoredCarDb != null) {
                CarDb.Clear();
                foreach (var car in restoredCarDb.OrderBy(x => x.Name)) {
                    CarDb.Add(car);
                    SaveCarToFile(car);
                }
            }
            else {
                throw new InvalidOperationException("Failed to deserialize the backup file.");
            }
        }

        public void RecalculateAllRatingsScale(int oldScale, int newScale, RatingRoundingMode roundingMode) {
            if (oldScale == newScale || oldScale == 0) {
                return;
            }

            CreateBackupOfCarDb();

            foreach (var car in CarDb) {
                if (car?.Ratings == null) continue;

                try {
                    car.Ratings.CornerHandling = RecalculateSingleRating(car.Ratings.CornerHandling, oldScale, newScale, roundingMode);
                    car.Ratings.Brakes = RecalculateSingleRating(car.Ratings.Brakes, oldScale, newScale, roundingMode);
                    car.Ratings.Realism = RecalculateSingleRating(car.Ratings.Realism, oldScale, newScale, roundingMode);
                    car.Ratings.Sound = RecalculateSingleRating(car.Ratings.Sound, oldScale, newScale, roundingMode);
                    car.Ratings.ExteriorQuality = RecalculateSingleRating(car.Ratings.ExteriorQuality, oldScale, newScale, roundingMode);
                    car.Ratings.InteriorQuality = RecalculateSingleRating(car.Ratings.InteriorQuality, oldScale, newScale, roundingMode);
                    car.Ratings.ForceFeedbackQuality = RecalculateSingleRating(car.Ratings.ForceFeedbackQuality, oldScale, newScale, roundingMode);
                    car.Ratings.FunFactor = RecalculateSingleRating(car.Ratings.FunFactor, oldScale, newScale, roundingMode);
                    SaveCarToFile(car);
                }
                catch (Exception ex) {
                    ErrorLogger.LogError("RecalculateRatingScale", new Exception($"Failed to recalculate ratings for car {car.Name ?? car.FolderName ?? "UNKNOWN"}: {ex.Message}", ex));
                }
            }

            _modifiedCars.Clear();
        }

        private static double RecalculateSingleRating(double currentRating, int oldScale, int newScale, RatingRoundingMode roundingMode) {
            if (oldScale <= 0) return 0;
            if (currentRating <= 0) return 0;

            currentRating = Math.Min(currentRating, oldScale);

            double normalizedRating = currentRating / oldScale;
            double newRatingRaw = normalizedRating * newScale;

            var finalRating = roundingMode == RatingRoundingMode.RoundUp ? Math.Ceiling(newRatingRaw) : Math.Floor(newRatingRaw);

            // Ensure the rating doesn't exceed the new maximum (due to rounding up) or go below 0
            return Math.Clamp(finalRating, 0, newScale);
        }
    }
}
