using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Ac.Ratings.Core;
using Ac.Ratings.Model;
using Ac.Ratings.Services.MainView;
using Ac.Ratings.Theme.ModernUI.Controls;

namespace Ac.Ratings.ViewModel {
    public class MainViewModel : Core.ViewModel {
        private ObservableCollection<Car> _carDb;
        private Car _selectedCar;
        private string _engineStats;
        private string _drivetrainStats;
        private string _gearboxStats;
        private string _searchText = string.Empty;
        private BitmapImage _carImageSource;
        private ObservableCollection<SkinPreview> _skinPreviews;

        private ObservableCollection<SelectableItem> _availableAuthors;
        private ObservableCollection<SelectableItem> _availableClasses;
        private ObservableCollection<string> _selectedAuthors;
        private ObservableCollection<string> _selectedClasses;
        private double _minCornerHandling;
        private double _minBrakes;
        private double _minRealism;
        private double _minSound;
        private double _minExteriorQuality;
        private double _minInteriorQuality;
        private double _minForceFeedbackQuality;
        private double _minFunFactor;
        private double _minAverageRating;
        private string _gearboxFilter;
        private string _drivetrainFilter;

        public ObservableCollection<Car> CarDb {
            get => _carDb;
            set => SetField(ref _carDb, value);
        }

        public Car SelectedCar {
            get => _selectedCar;
            set {
                if (SetField(ref _selectedCar, value)) {
                    UpdateCarDisplayData();
                }
            }
        }

        public string EngineStats {
            get => _engineStats;
            set => SetField(ref _engineStats, value);
        }

        public string DrivetrainStats {
            get => _drivetrainStats;
            set => SetField(ref _drivetrainStats, value);
        }

        public string GearboxStats {
            get => _gearboxStats;
            set => SetField(ref _gearboxStats, value);
        }

        public BitmapImage CarImageSource {
            get => _carImageSource;
            set => SetField(ref _carImageSource, value);
        }

        public ObservableCollection<SkinPreview> SkinPreviews {
            get => _skinPreviews;
            set => SetField(ref _skinPreviews, value);
        }

        public string SearchText {
            get => _searchText;
            set {
                if (SetField(ref _searchText, value)) {
                    CarView?.Refresh();
                    SelectFirstFilteredCar();
                }
            }
        }

        public ObservableCollection<SelectableItem> AvailableAuthors {
            get => _availableAuthors;
            set => SetField(ref _availableAuthors, value);
        }

        public ObservableCollection<SelectableItem> AvailableClasses {
            get => _availableClasses;
            set => SetField(ref _availableClasses, value);
        }


        public ObservableCollection<string> SelectedAuthors {
            get => _selectedAuthors;
            set => SetField(ref _selectedAuthors, value);
        }

        public ObservableCollection<string> SelectedClasses {
            get => _selectedClasses;
            set => SetField(ref _selectedClasses, value);
        }

        public double MinCornerHandling {
            get => _minCornerHandling;
            set => SetField(ref _minCornerHandling, value);
        }

        public double MinBrakes {
            get => _minBrakes;
            set => SetField(ref _minBrakes, value);
        }

        public double MinRealism {
            get => _minRealism;
            set => SetField(ref _minRealism, value);
        }

        public double MinSound {
            get => _minSound;
            set => SetField(ref _minSound, value);
        }

        public double MinExteriorQuality {
            get => _minExteriorQuality;
            set => SetField(ref _minExteriorQuality, value);
        }

        public double MinInteriorQuality {
            get => _minInteriorQuality;
            set => SetField(ref _minInteriorQuality, value);
        }

        public double MinForceFeedbackQuality {
            get => _minForceFeedbackQuality;
            set => SetField(ref _minForceFeedbackQuality, value);
        }

        public double MinFunFactor {
            get => _minFunFactor;
            set => SetField(ref _minFunFactor, value);
        }

        public double MinAverageRating {
            get => _minAverageRating;
            set => SetField(ref _minAverageRating, value);
        }

        public string GearboxFilter {
            get => _gearboxFilter;
            set => SetField(ref _gearboxFilter, value);
        }

        public string DrivetrainFilter {
            get => _drivetrainFilter;
            set => SetField(ref _drivetrainFilter, value);
        }


        // Used for binding in view
        public IEnumerable<string> GearboxOptions => new[] { "Any", "Manual", "Automatic" };
        public IEnumerable<string> DrivetrainOptions => new[] { "Any", "RWD", "FWD", "AWD" };


        public ICollectionView CarView { get; private set; }
        public RelayCommand ClearRatingsCommand { get; }
        public RelayCommand ClearExtraFeaturesCommand { get; }
        public RelayCommand<string> SelectSkinCommand { get; }
        public RelayCommand ResetFiltersCommand { get; }

        public RelayCommand ApplyFiltersCommand { get; }
        public RelayCommand ResetViewFiltersCommand { get; }

        public MainViewModel() {
            CarDb = new ObservableCollection<Car>(CarDataService.LoadCarDatabase());

            _availableAuthors = GetAuthors();
            _availableClasses = GetClasses();
            _selectedAuthors = new ObservableCollection<string>();
            _selectedClasses = new ObservableCollection<string>();

            CarView = CollectionViewSource.GetDefaultView(CarDb);
            CarView.Filter = FilterCar;

            ClearRatingsCommand = new RelayCommand(ClearRatings);
            ClearExtraFeaturesCommand = new RelayCommand(ClearExtraFeatures);
            SelectSkinCommand = new RelayCommand<string>(SelectSkin);
            ResetFiltersCommand = new RelayCommand(ResetFilters);
            ApplyFiltersCommand = new RelayCommand(ApplyFilters);
            ResetViewFiltersCommand = new RelayCommand(ResetViewFilters);

            _skinPreviews = new ObservableCollection<SkinPreview>();
            if (CarDb.Count > 0) {
                SelectedCar = CarDb[0];
            }
        }


        private bool FilterCar(object obj) {
            if (obj is not Car car) return false;

            if (!string.IsNullOrWhiteSpace(SearchText) && !car.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) return false;
            if (SelectedAuthors.Any() && !SelectedAuthors.Contains(car.Author)) return false;
            if (SelectedClasses.Any() && !SelectedClasses.Contains(car.Class)) return false;

            if(MinCornerHandling > car.Ratings.CornerHandling) return false;
            if(MinBrakes > car.Ratings.Brakes) return false;
            if(MinRealism > car.Ratings.Realism) return false;
            if(MinSound > car.Ratings.Sound) return false;
            if(MinExteriorQuality > car.Ratings.ExteriorQuality) return false;
            if(MinInteriorQuality > car.Ratings.InteriorQuality) return false;
            if(MinForceFeedbackQuality > car.Ratings.ForceFeedbackQuality) return false;
            if(MinFunFactor > car.Ratings.FunFactor) return false;
            if(MinAverageRating > car.Ratings.AverageRating) return false;

            bool isManual = car.Data.SupportsShifter;
            if (GearboxFilter == "Manual" && !isManual) return false;
            if (GearboxFilter == "Automatic" && isManual) return false;

            string drivetrain = car.Data.TractionType?.ToLower() ?? string.Empty;
            if (DrivetrainFilter == "RWD" && !drivetrain.Contains("rwd")) return false;
            if (DrivetrainFilter == "FWD" && !drivetrain.Contains("fwd")) return false;
            if (DrivetrainFilter == "AWD" && !drivetrain.Contains("awd")) return false;

            return true;
        }

        private void ApplyFilters() {
            SelectedAuthors.Clear();
            foreach (var author in AvailableAuthors.Where(a => a.IsSelected)) {
                SelectedAuthors.Add(author.Name);
            }

            SelectedClasses.Clear();
            foreach (var classItem in AvailableClasses.Where(c => c.IsSelected)) {
                SelectedClasses.Add(classItem.Name);
            }

            CarView?.Refresh();
            SelectFirstFilteredCar();

            if (Application.Current.MainWindow is MainWindow mainWindow) {
                if (mainWindow.Template?.FindName("ContentFrame", mainWindow) is ModernFrame frame) {
                    frame.Source = new Uri("/Theme/Components/Home.xaml", UriKind.Relative);
                }
            }
        }

        private ObservableCollection<SelectableItem> GetAuthors() {
            return new ObservableCollection<SelectableItem>(
                _carDb.Select(c => c.Author)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()
                    .OrderBy(a => a)
                    .Select(a => new SelectableItem { Name = a }));
        }

        private ObservableCollection<SelectableItem> GetClasses() {
            return new ObservableCollection<SelectableItem>(
                _carDb.Select(c => c.Class)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()
                    .OrderBy(c => c)
                    .Select(c => new SelectableItem { Name = c }));
        }

        private void ClearRatings() {
            if (SelectedCar != null) {
                CarRatingService.ResetRatingValues(SelectedCar);
            }
        }

        private void ClearExtraFeatures() {
            if (SelectedCar != null) {
                CarRatingService.ResetExtraFeatureValues(SelectedCar);
            }
        }

        public void ResetViewFilters() {
            foreach (var author in AvailableAuthors) author.IsSelected = false;
            foreach (var classItem in AvailableClasses) classItem.IsSelected = false;
            SelectedAuthors.Clear();
            SelectedClasses.Clear();

            MinCornerHandling = 0;
            MinBrakes = 0;
            MinRealism = 0;
            MinSound = 0;
            MinExteriorQuality = 0;
            MinInteriorQuality = 0;
            MinForceFeedbackQuality = 0;
            MinFunFactor = 0;
            MinAverageRating = 0;

            GearboxFilter = "Any";
            DrivetrainFilter = "Any";

            CarView?.Refresh();
            SelectFirstFilteredCar();
        }

        private void ResetFilters() {
            SearchText = string.Empty;
            ResetViewFilters();
        }

        public void SelectFirstFilteredCar() {
            SelectedCar = CarView.Cast<Car>().FirstOrDefault();
        }

        private void UpdateCarDisplayData() {
            if (SelectedCar != null) {
                EngineStats = CarDisplayService.ShowCarEngineStats(SelectedCar);
                DrivetrainStats = CarDisplayService.ShowCarDriveTrain(SelectedCar);
                GearboxStats = CarDisplayService.ShowCarGearbox(SelectedCar);
                LoadSkinPreviews();
            }
            else {
                EngineStats = string.Empty;
                DrivetrainStats = string.Empty;
                GearboxStats = string.Empty;
                CarImageSource = null;
                SkinPreviews?.Clear();
            }
        }

        private void LoadSkinPreviews() {
            if (string.IsNullOrEmpty(SelectedCar?.FolderPath)) return;

            var skinsDirectory = Path.Combine(SelectedCar.FolderPath, "skins");
            if (!Directory.Exists(skinsDirectory)) return;

            var skinDirectories = Directory.GetDirectories(skinsDirectory);
            SkinPreviews.Clear();

            foreach (var dir in skinDirectories) {
                var previewFilePath = Path.Combine(dir, "preview.jpg");
                var liveryFilePath = Path.Combine(dir, "livery.png");

                if (File.Exists(previewFilePath) && File.Exists(liveryFilePath)) {
                    try {
                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri(liveryFilePath, UriKind.Absolute);
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();

                        SkinPreviews.Add(new SkinPreview {
                            LiveryImage = bitmapImage,
                            PreviewPath = previewFilePath
                        });
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"Failed to load skin: {ex.Message}");
                    }
                }
            }

            if (SkinPreviews.Any()) {
                SelectSkin(SkinPreviews[0].PreviewPath);
            }
            else {
                CarImageSource = null;
            }
        }

        private void SelectSkin(string previewPath) {
            if (File.Exists(previewPath)) {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(previewPath, UriKind.Absolute);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();
                CarImageSource = image;
            }
        }

        public class SkinPreview {
            public BitmapImage LiveryImage { get; set; }
            public string PreviewPath { get; set; }
        }

        public class SelectableItem : ObservableObject {
            private bool _isSelected;
            private string _name;

            public bool IsSelected {
                get => _isSelected;
                set => SetField(ref _isSelected, value);
            }

            public string Name {
                get => _name;
                set => SetField(ref _name, value);
            }
        }
    }
}
