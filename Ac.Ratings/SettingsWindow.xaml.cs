using System.Collections.ObjectModel;
using Ac.Ratings.Core;
using Ac.Ratings.Model;
using Ac.Ratings.Theme.ModernUI.Controls;
using Ac.Ratings.ViewModel;

namespace Ac.Ratings {
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : ModernWindowBase {
        public SettingsViewModel ViewModel { get; }

        public SettingsWindow(ObservableCollection<Car> carDb) {
            InitializeComponent();
            var dialogService = new ModernDialogService(this);
            ViewModel = new SettingsViewModel(dialogService);
            ViewModel.SetCarDb(carDb);
            DataContext = ViewModel;
        }
    }
}
