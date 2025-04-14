using System.Windows;
using Ac.Ratings.Services.Interfaces;
using Ac.Ratings.Theme.ModernUI.Controls;
using Ac.Ratings.ViewModel;

namespace Ac.Ratings.View {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindowBase {
        private readonly MainViewModel _viewModel;
        private readonly ICarDataService _carDataService;

        public MainWindow(MainViewModel viewModel, ICarDataService carDataService) {
            InitializeComponent();
            try {
                _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
                _carDataService = carDataService ?? throw new ArgumentNullException(nameof(carDataService));
                DataContext = _viewModel;
            }
            catch (Exception ex) {
                MessageBox.Show($"Failed to load cars: {ex.Message}");
            }
        }

        private void CreateBackupOfCarDb() {
            try {
                _carDataService.CreateBackupOfCarDb();
            }
            catch (Exception ex) {
                MessageBox.Show($"Error creating backup: {ex.Message}", "Backup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            _carDataService.SaveModifiedCars();
            CreateBackupOfCarDb(); /*--Comment out only while testing other features*/
        }
    }
}