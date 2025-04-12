using System.Collections.ObjectModel;
using Ac.Ratings.Model;

namespace Ac.Ratings.Services.Interfaces;

public interface ICarDataManager {
    void MarkCarAsModified(Car car);
    void SaveModifiedCars();
    void SaveCarToFile(Car car);
    void CreateBackupOfCarDb(ObservableCollection<Car> cars);
    void ResetAllRatingsInDatabase(ObservableCollection<Car> cars);
    void ResetAllExtraFeaturesInDatabase(ObservableCollection<Car> cars);
    List<Car> RestoreCarDbFromBackup(string backupFilePath);
}