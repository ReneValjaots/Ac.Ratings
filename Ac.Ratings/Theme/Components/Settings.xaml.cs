using Ac.Ratings.Core;
using Ac.Ratings.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Ac.Ratings.Theme.Components {
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
