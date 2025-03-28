using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Ac.Ratings.Core;
using Ac.Ratings.Model;
using Ac.Ratings.Services.MainView;

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
        public FilterViewModel _filterViewModel;

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

        public List<string> Authors { get; }
        public List<string> Classes { get; }

        public string SearchText {
            get => _searchText;
            set {
                if (SetField(ref _searchText, value)) {
                    CarView?.Refresh();
                    SelectFirstFilteredCar();
                }
            }
        }

        public ICollectionView CarView { get; private set; }

        public RelayCommand ClearRatingsCommand { get; }
        public RelayCommand ClearExtraFeaturesCommand { get; }
        public RelayCommand<string> SelectSkinCommand { get; }
        public RelayCommand ResetFiltersCommand { get; }

        public MainViewModel() {
            try {
                CarDb = new ObservableCollection<Car>(CarDataService.LoadCarDatabase());
                Authors = CarDataService.GetDistinctAuthors(CarDb);
                Classes = CarDataService.GetDistinctClasses(CarDb);
                _filterViewModel = new FilterViewModel(CarDb);

                CarView = CollectionViewSource.GetDefaultView(CarDb);
                CarView.Filter = obj => {
                    if (obj is Car car) {
                        return FilterCar(car);
                    }

                    return false;
                };

                ClearRatingsCommand = new RelayCommand(ClearRatings);
                ClearExtraFeaturesCommand = new RelayCommand(ClearExtraFeatures);
                SelectSkinCommand = new RelayCommand<string>(SelectSkin);
                ResetFiltersCommand = new RelayCommand(ResetFilters);

                _skinPreviews = new ObservableCollection<SkinPreview>();
                if (CarDb.Count > 0) {
                    SelectedCar = CarDb[0];
                }
            }
            // ReSharper disable once RedundantCatchClause
            catch (Exception ex) {
                throw; // Re-throw to let the View handle UI notification
            }
        }


        private bool FilterCar(Car car) {
            if (!string.IsNullOrWhiteSpace(SearchText) && !car.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) return false;
            if (_filterViewModel.SelectedAuthors.Any() && !_filterViewModel.SelectedAuthors.Contains(car.Author)) return false;
            if (_filterViewModel.SelectedClasses.Any() && !_filterViewModel.SelectedClasses.Contains(car.Class)) return false;

            if(_filterViewModel.MinCornerHandling > car.Ratings.CornerHandling) return false;
            if(_filterViewModel.MinBrakes > car.Ratings.Brakes) return false;
            if(_filterViewModel.MinRealism > car.Ratings.Realism) return false;
            if(_filterViewModel.MinSound > car.Ratings.Sound) return false;
            if(_filterViewModel.MinExteriorQuality > car.Ratings.ExteriorQuality) return false;
            if(_filterViewModel.MinInteriorQuality > car.Ratings.InteriorQuality) return false;
            if(_filterViewModel.MinForceFeedbackQuality > car.Ratings.ForceFeedbackQuality) return false;
            if(_filterViewModel.MinFunFactor > car.Ratings.FunFactor) return false;
            if(_filterViewModel.MinAverageRating > car.Ratings.AverageRating) return false;

            bool isManual = car.Data.SupportsShifter;
            if (_filterViewModel.GearboxFilter == "Manual" && !isManual) return false;
            if (_filterViewModel.GearboxFilter == "Automatic" && isManual) return false;

            string drivetrain = car.Data.TractionType?.ToLower() ?? "";
            if (_filterViewModel.DrivetrainFilter == "RWD" && !drivetrain.Contains("rwd")) return false;
            if (_filterViewModel.DrivetrainFilter == "FWD" && !drivetrain.Contains("fwd")) return false;
            if (_filterViewModel.DrivetrainFilter == "AWD" && !drivetrain.Contains("awd")) return false;

            return true;
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

        private void ResetFilters() {
            SearchText = string.Empty;
            _filterViewModel.ResetFilters();
            CarView?.Refresh();
            SelectFirstFilteredCar();
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
    }
}
