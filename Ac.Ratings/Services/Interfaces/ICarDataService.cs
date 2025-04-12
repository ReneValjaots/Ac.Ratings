using System.Collections.ObjectModel;
using Ac.Ratings.Model;

namespace Ac.Ratings.Services.Interfaces;

public interface ICarDataService {
    List<Car> LoadCarDatabase();
    ObservableCollection<Car> CarDb { get; }
    void ResetAllRatings();
    void ResetAllExtraFeatures();
    Car RestoreCarDbFromBackup(string backupFilePath);
}