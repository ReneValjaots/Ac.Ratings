using Ac.Ratings.Model;

namespace Ac.Ratings.Services {
    public class CarDataService : ICarDataService{
        public List<Car> LoadCarDatabase() {
            if (string.IsNullOrEmpty(ConfigManager.AcRootFolder))
                return new List<Car>();

            var factory = new CarFactory(ConfigManager.AcRootFolder, ConfigManager.CarsRootFolder);
            return factory.InitializeCars();
        }
    }

    public interface ICarDataService {
        List<Car> LoadCarDatabase();
    }
}
