using System.Windows.Controls;
using Ac.Ratings.ViewModel;

namespace Ac.Ratings.Theme.Components
{
    /// <summary>
    /// Interaction logic for Appearance.xaml
    /// </summary>
    public partial class Appearance : UserControl
    {
        public Appearance()
        {
            InitializeComponent();
            DataContext = new AppearanceViewModel();
        }
    }
}
