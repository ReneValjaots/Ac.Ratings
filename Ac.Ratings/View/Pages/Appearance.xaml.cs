using System.Windows;
using System.Windows.Controls;

namespace Ac.Ratings.View.Pages {
    /// <summary>
    /// Interaction logic for Appearance.xaml
    /// </summary>
    public partial class Appearance : UserControl {
        public Appearance() {
            InitializeComponent();
            DataContext = ((App)Application.Current).AppearanceViewModel;
        }
    }
}
