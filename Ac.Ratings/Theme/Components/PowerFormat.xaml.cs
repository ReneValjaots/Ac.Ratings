using System.Windows;
using System.Windows.Controls;

namespace Ac.Ratings.Theme.Components
{
    /// <summary>
    /// Interaction logic for PowerFormat.xaml
    /// </summary>
    public partial class PowerFormat : UserControl
    {
        public PowerFormat()
        {
            InitializeComponent();
        }

        private void OnSaveClick(object sender, System.Windows.RoutedEventArgs e) {
            var window = Window.GetWindow(this) as SettingsWindow;
            if (window != null) {
                window.OnSaveClick(sender, e);
            }
        }
    }
}
