using System.Windows;
using System.Windows.Controls;
using Ac.Ratings.Core;
using Ac.Ratings.ViewModel;

namespace Ac.Ratings.View.Pages {
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl {
        public SettingsViewModel ViewModel { get; }

        public Settings() {
            InitializeComponent();
            var mainWindow = Application.Current.MainWindow as MainWindow;
            var dialogService = new ModernDialogService(mainWindow);
            ViewModel = new SettingsViewModel(dialogService);
            ViewModel.SetCarDb(mainWindow?._viewModel?.CarDb);
            DataContext = ViewModel;
        }
    }
}
