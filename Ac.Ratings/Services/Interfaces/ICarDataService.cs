using System.Collections.ObjectModel;
using Ac.Ratings.Model;

namespace Ac.Ratings.Services.Interfaces;

public interface ICarDataService {
    ObservableCollection<Car> CarDb { get; }
    void MarkCarAsModified(Car car);
    void SaveModifiedCars();
    void CreateBackupOfCarDb();
    void ResetAllRatings();
    void ResetAllExtraFeatures();
    Car RestoreCarDbFromBackup(string backupFilePath);
}