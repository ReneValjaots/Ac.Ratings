using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Ac.Ratings.Core;
using Ac.Ratings.Model;
using Ac.Ratings.Services.MainView;

namespace Ac.Ratings.ViewModel {
    public class MainViewModel : ObservableObject {
        private ObservableCollection<Car> _carDb;
        private Car _selectedCar;
        private string _engineStats;
        private string _drivetrainStats;
        private string _gearboxStats;
        private string _searchText = string.Empty;
        private BitmapImage _carImageSource;
        private ObservableCollection<SkinPreview> _skinPreviews;
        private FilterViewModel _filterViewModel;

        public ObservableCollection<Car> CarDb {
            get => _carDb;
            set => SetField(ref _carDb, value);
        }

        public Car SelectedCar {
            get => _selectedCar;
            set {
                if (SetField(ref _selectedCar, value)) {
                    UpdateCarData();
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

        public ICommand ClearRatingsCommand { get; }
        public ICommand ClearExtraFeaturesCommand { get; }
        public ICommand SelectSkinCommand { get; }
        public ICommand OpenFilterCommand { get; }
        public ICommand ResetFiltersCommand { get; }

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
                OpenFilterCommand = new RelayCommand(OpenFilterDialog);
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
            // Search filter
            if (!string.IsNullOrWhiteSpace(SearchText) && !car.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                return false;

            // Author filter
            if (_filterViewModel.SelectedAuthors.Any() && !_filterViewModel.SelectedAuthors.Contains(car.Author))
                return false;

            // Class filter
            if (_filterViewModel.SelectedClasses.Any() && !_filterViewModel.SelectedClasses.Contains(car.Class))
                return false;

            // Ratings filter
            if (_filterViewModel.MinCornerHandling > 0 && (car.Ratings?.CornerHandling ?? 0) < _filterViewModel.MinCornerHandling)
                return false;
            if (_filterViewModel.MinBraking > 0 && (car.Ratings?.Brakes ?? 0) < _filterViewModel.MinBraking)
                return false;
            if (_filterViewModel.MinRealism > 0 && (car.Ratings?.Realism ?? 0) < _filterViewModel.MinRealism)
                return false;
            if (_filterViewModel.MinSound > 0 && (car.Ratings?.Sound ?? 0) < _filterViewModel.MinSound)
                return false;
            if (_filterViewModel.MinExteriorQuality > 0 && (car.Ratings?.ExteriorQuality ?? 0) < _filterViewModel.MinExteriorQuality)
                return false;
            if (_filterViewModel.MinInteriorQuality > 0 && (car.Ratings?.InteriorQuality ?? 0) < _filterViewModel.MinInteriorQuality)
                return false;
            if (_filterViewModel.MinForceFeedbackQuality > 0 && (car.Ratings?.ForceFeedbackQuality ?? 0) < _filterViewModel.MinForceFeedbackQuality)
                return false;
            if (_filterViewModel.MinFunFactor > 0 && (car.Ratings?.FunFactor ?? 0) < _filterViewModel.MinFunFactor)
                return false;
            if (_filterViewModel.MinAverageRating > 0 && (car.Ratings?.AverageRating ?? 0) < _filterViewModel.MinAverageRating)
                return false;
            bool isManual = car.Data.SupportsShifter;
            if (_filterViewModel.IsAutomatic && isManual) return false;
            if (_filterViewModel.IsManual && !isManual) return false;

            string drivetrain = car.Data.TractionType?.ToLower() ?? "";
            if (_filterViewModel.IsRwd && !drivetrain.Contains("rwd")) return false;
            if (_filterViewModel.IsFwd && !drivetrain.Contains("fwd")) return false;
            if (_filterViewModel.IsAwd && !drivetrain.Contains("awd")) return false;
            
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

        private void OpenFilterDialog() {
            var dialog = new FilterWindow { DataContext = _filterViewModel, Owner = Application.Current.MainWindow };
            dialog.ShowDialog();
            CarView?.Refresh();
            SelectFirstFilteredCar();
        }

        private void ResetFilters() {
            SearchText = string.Empty;
            _filterViewModel.ResetFilters();
            CarView?.Refresh();
            SelectFirstFilteredCar();
        }

        private void SelectFirstFilteredCar() {
            SelectedCar = CarView.Cast<Car>().FirstOrDefault();
        }

        private void UpdateCarData() {
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
