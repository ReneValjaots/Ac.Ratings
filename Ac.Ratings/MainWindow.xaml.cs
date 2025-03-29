using System.Windows;
using Ac.Ratings.Services;
using Ac.Ratings.Theme.ModernUI.Controls;
using Ac.Ratings.ViewModel;

namespace Ac.Ratings {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindowBase {
        public MainViewModel _viewModel;

        public MainWindow(MainViewModel viewModel) {
            InitializeComponent();
            try {
                _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
                DataContext = _viewModel;
            }
            catch (Exception ex) {
                MessageBox.Show($"Failed to load cars: {ex.Message}");
            }
        }

        private void CreateBackupOfCarDb() {
            try {
                CarDataManager.CreateBackupOfCarDb(_viewModel.CarDb);
            }
            catch (Exception ex) {
                MessageBox.Show($"Error creating backup: {ex.Message}", "Backup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            CarDataManager.SaveModifiedCars();
            //CreateBackupOfCarDb(); -- Comment out only while testing other features
        }
    }
}