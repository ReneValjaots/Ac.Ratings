using System.Windows;
using System.Windows.Controls;

namespace Ac.Ratings.View.Pages {
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl {
        public Settings() {
            InitializeComponent();
            var viewModel = ((App)Application.Current).SettingsViewModel;
            DataContext = viewModel;
        }
    }
}
