using System.Windows;
using System.Windows.Controls;

namespace Ac.Ratings.Theme.Components {
    /// <summary>
    /// Interaction logic for Filter.xaml
    /// </summary>
    public partial class Filter : UserControl {
        public Filter() {
            InitializeComponent();
            var mainWindow = Application.Current.MainWindow as MainWindow;
            DataContext = mainWindow?._viewModel?._filterViewModel;
        }
    }
}
