using Ac.Ratings.Model;
using System.Collections.ObjectModel;

namespace Ac.Ratings.Services {
    public class CarDataService : ICarDataService{
        public ObservableCollection<Car> CarDb { get; }

        public CarDataService() {
            CarDb = new ObservableCollection<Car>(LoadCarDatabase());
        }

        public List<Car> LoadCarDatabase() {
            if (string.IsNullOrEmpty(ConfigManager.AcRootFolder))
                return new List<Car>();

            var factory = new CarFactory(ConfigManager.AcRootFolder, ConfigManager.CarsRootFolder);
            return factory.InitializeCars();
        }

        public void ResetAllRatings() {
            CarDataManager.ResetAllRatingsInDatabase(CarDb);
        }

        public void ResetAllExtraFeatures() {
            CarDataManager.ResetAllExtraFeaturesInDatabase(CarDb);
        }

        public Car RestoreCarDbFromBackup(string backupFilePath) {
            var restoredCarDb = CarDataManager.RestoreCarDbFromBackup(backupFilePath);
            if (restoredCarDb != null) {
                CarDb.Clear();
                foreach (var car in restoredCarDb) {
                    CarDb.Add(car);
                    CarDataManager.SaveCarToFile(car);
                }

                return CarDb.FirstOrDefault();
            }

            return null;
        }
   
    }

    public interface ICarDataService {
        List<Car> LoadCarDatabase();
        ObservableCollection<Car> CarDb { get; }
        void ResetAllRatings();
        void ResetAllExtraFeatures();
        Car RestoreCarDbFromBackup(string backupFilePath);
    }
}
