using Ac.Ratings.Core;
using Ac.Ratings.Model;
using Ac.Ratings.Theme.ModernUI.Controls;
using System.Collections.ObjectModel;
using System.Windows;

namespace Ac.Ratings.ViewModel {
    public class FilterViewModel : Core.ViewModel {
        private readonly ObservableCollection<Car> _cars;
        private ObservableCollection<AuthorItem> _availableAuthors;
        private ObservableCollection<ClassItem> _availableClasses;
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

        public FilterViewModel(ObservableCollection<Car> cars) {
            _cars = cars;
            _availableAuthors = GetAuthors();
            _availableClasses = GetClasses();
            _selectedAuthors = new ObservableCollection<string>();
            _selectedClasses = new ObservableCollection<string>();
    

            ApplyFiltersCommand = new RelayCommand(ApplyFilters);
            ResetFiltersCommand = new RelayCommand(ResetFilters);
        }

        public ObservableCollection<AuthorItem> AvailableAuthors {
            get => _availableAuthors;
            set => SetField(ref _availableAuthors, value);
        }

        public ObservableCollection<ClassItem> AvailableClasses {
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

        public RelayCommand ApplyFiltersCommand { get; }
        public RelayCommand ResetFiltersCommand { get; }

        private ObservableCollection<AuthorItem> GetAuthors() {
            return new ObservableCollection<AuthorItem>(
                _cars.Select(c => c.Author)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()
                    .OrderBy(a => a)
                    .Select(a => new AuthorItem { Name = a }));
        }

        private ObservableCollection<ClassItem> GetClasses() {
            return new ObservableCollection<ClassItem>(
                _cars.Select(c => c.Class)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()
                    .OrderBy(c => c)
                    .Select(c => new ClassItem { Name = c }));
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

            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?._viewModel.CarView?.Refresh();
            mainWindow?._viewModel.SelectFirstFilteredCar();

            if (mainWindow != null) {
                if (mainWindow.Template?.FindName("ContentFrame", mainWindow) is ModernFrame frame) {
                    frame.Source = new Uri("/Theme/Components/Home.xaml", UriKind.Relative);
                }
            }
        }

        public void ResetFilters() {
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

            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?._viewModel.CarView?.Refresh();
            mainWindow?._viewModel.SelectFirstFilteredCar();
        }
    }

    public class AuthorItem : ObservableObject {
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

    public class ClassItem : ObservableObject {
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
