using Ac.Ratings.Model;
using Ac.Ratings.Services.Interfaces;
using System.Collections.ObjectModel;

namespace Ac.Ratings.Services {
    public class CarDataService : ICarDataService{
        private readonly ICarDataManager _carDataManager;
        public ObservableCollection<Car> CarDb { get; }

        public CarDataService(ICarDataManager carDataManager) {
            _carDataManager = carDataManager ?? throw new ArgumentNullException(nameof(carDataManager));
            CarDb = new ObservableCollection<Car>(LoadCarDatabase());
        }

        public List<Car> LoadCarDatabase() {
            if (string.IsNullOrEmpty(ConfigManager.AcRootFolder))
                return new List<Car>();

            var factory = new CarFactory(ConfigManager.AcRootFolder, ConfigManager.CarsRootFolder);
            return factory.InitializeCars();
        }

        public void ResetAllRatings() {
            _carDataManager.ResetAllRatingsInDatabase(CarDb);
        }

        public void ResetAllExtraFeatures() {
            _carDataManager.ResetAllExtraFeaturesInDatabase(CarDb);
        }

        public Car RestoreCarDbFromBackup(string backupFilePath) {
            var restoredCarDb = _carDataManager.RestoreCarDbFromBackup(backupFilePath);
            if (restoredCarDb != null) {
                CarDb.Clear();
                foreach (var car in restoredCarDb) {
                    CarDb.Add(car);
                    _carDataManager.SaveCarToFile(car);
                }

                return CarDb.FirstOrDefault();
            }

            return null;
        }
    }
}
