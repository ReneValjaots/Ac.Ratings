using System.Windows;
using System.Windows.Controls;

namespace Ac.Ratings.View.Pages {
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl {
        public Home() {
            InitializeComponent();
            DataContext = (Application.Current.MainWindow as MainWindow)?._viewModel;
        }
    }
}
