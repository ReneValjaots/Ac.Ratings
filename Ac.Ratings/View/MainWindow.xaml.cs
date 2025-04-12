using System.Windows;
using Ac.Ratings.Services;
using Ac.Ratings.Services.Interfaces;
using Ac.Ratings.Theme.ModernUI.Controls;
using Ac.Ratings.ViewModel;

namespace Ac.Ratings.View {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindowBase {
        private readonly MainViewModel _viewModel;
        private readonly ICarDataManager _carDataManager;

        public MainWindow(MainViewModel viewModel, ICarDataManager carDataManager) {
            InitializeComponent();
            try {
                _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
                _carDataManager = carDataManager ?? throw new ArgumentNullException(nameof(carDataManager));
                DataContext = _viewModel;
            }
            catch (Exception ex) {
                MessageBox.Show($"Failed to load cars: {ex.Message}");
            }
        }

        private void CreateBackupOfCarDb() {
            try {
                _carDataManager.CreateBackupOfCarDb(_viewModel.CarDb);
            }
            catch (Exception ex) {
                MessageBox.Show($"Error creating backup: {ex.Message}", "Backup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            _carDataManager.SaveModifiedCars();
            CreateBackupOfCarDb(); /*--Comment out only while testing other features*/
        }
    }
}