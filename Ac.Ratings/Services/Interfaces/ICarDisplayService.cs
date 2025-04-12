using Ac.Ratings.Model;

namespace Ac.Ratings.Services.Interfaces;

public interface ICarDisplayService {
    string ShowCarEngineStats(Car selectedCar);
    string ShowCarDriveTrain(Car selectedCar);
    string ShowCarGearbox(Car selectedCar);
}