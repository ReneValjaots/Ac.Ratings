using Ac.Ratings.Core;
using Ac.Ratings.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Ac.Ratings.ViewModel {
    public class FilterViewModel : ObservableObject {
        private readonly ObservableCollection<Car> _cars;
        private ObservableCollection<AuthorItem> _availableAuthors;
        private ObservableCollection<ClassItem> _availableClasses;
        private ObservableCollection<string> _selectedAuthors;
        private ObservableCollection<string> _selectedClasses;
        private ObservableCollection<FilterCriteria> _ratingFilters;
        private string _gearboxFilter;
        private string _drivetrainFilter;

        public FilterViewModel(ObservableCollection<Car> cars) {
            _cars = cars;
            _availableAuthors = GetAuthors();
            _availableClasses = GetClasses();
            _selectedAuthors = new ObservableCollection<string>();
            _selectedClasses = new ObservableCollection<string>();
            _ratingFilters = new ObservableCollection<FilterCriteria> {
                new() { PropertyName = nameof(CarRatings.CornerHandling), DisplayName = "Corner Handling" },
                new() { PropertyName = nameof(CarRatings.Brakes), DisplayName = "Braking" },
                new() { PropertyName = nameof(CarRatings.Realism), DisplayName = "Realism" },
                new() { PropertyName = nameof(CarRatings.Sound), DisplayName = "Sound" },
                new() { PropertyName = nameof(CarRatings.ExteriorQuality), DisplayName = "Exterior Quality" },
                new() { PropertyName = nameof(CarRatings.InteriorQuality), DisplayName = "Interior Quality" },
                new() { PropertyName = nameof(CarRatings.ForceFeedbackQuality), DisplayName = "FFB Quality" },
                new() { PropertyName = nameof(CarRatings.FunFactor), DisplayName = "Fun Factor" },
                new() { PropertyName = nameof(CarRatings.AverageRating), DisplayName = "Average Rating" }
            };

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

        public ObservableCollection<FilterCriteria> RatingFilters {
            get => _ratingFilters;
            set => SetField(ref _ratingFilters, value);
        }

        public string GearboxFilter {
            get => _gearboxFilter;
            set => SetField(ref _gearboxFilter, value);
        }

        public string DrivetrainFilter {
            get => _drivetrainFilter;
            set => SetField(ref _drivetrainFilter, value);
        }

        public IEnumerable<string> GearboxOptions => new[] { "Any", "Manual", "Automatic" };
        public IEnumerable<string> DrivetrainOptions => new[] { "Any", "RWD", "FWD", "AWD" };

        public ICommand ApplyFiltersCommand { get; }
        public ICommand ResetFiltersCommand { get; }

        private ObservableCollection<AuthorItem> GetAuthors() {
            return new ObservableCollection<AuthorItem>(
                _cars.Select(c => c.Author)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()
                    .Select(a => new AuthorItem { Name = a }));
        }

        private ObservableCollection<ClassItem> GetClasses() {
            return new ObservableCollection<ClassItem>(
                _cars.Select(c => c.Class)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()
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

            var window = Application.Current.Windows.OfType<FilterWindow>().FirstOrDefault();
            window?.Close();
        }

        public void ResetFilters() {
            foreach (var author in AvailableAuthors) author.IsSelected = false;
            foreach (var classItem in AvailableClasses) classItem.IsSelected = false;
            SelectedAuthors.Clear();
            SelectedClasses.Clear();
            foreach (var filter in RatingFilters) filter.MinValue = 0;
            GearboxFilter = "Any";
            DrivetrainFilter = "Any";
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

    public class FilterCriteria : ObservableObject {
        private string _propertyName;
        private string _displayName;
        private int _minValue;

        public string PropertyName {
            get => _propertyName;
            set => SetField(ref _propertyName, value);
        }

        public string DisplayName {
            get => _displayName;
            set => SetField(ref _displayName, value);
        }

        public int MinValue {
            get => _minValue;
            set => SetField(ref _minValue, value);
        }
    }
}
