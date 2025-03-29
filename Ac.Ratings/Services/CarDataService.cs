using Ac.Ratings.Model;

namespace Ac.Ratings.Services {
    public static class CarDataService {
        public static List<Car> LoadCarDatabase() {
            if (string.IsNullOrEmpty(ConfigManager.AcRootFolder))
                return new List<Car>();

            var factory = new CarFactory(ConfigManager.AcRootFolder, ConfigManager.CarsRootFolder);
            return factory.InitializeCars();
        }
    }
}
