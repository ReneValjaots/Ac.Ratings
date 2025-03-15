using System.Windows;
using System.Windows.Controls;

namespace Ac.Ratings.Theme.Components
{
    /// <summary>
    /// Interaction logic for RestoreRatings.xaml
    /// </summary>
    public partial class RestoreRatings : UserControl
    {
        public RestoreRatings()
        {
            InitializeComponent();
        }

        private void RestoreBackupButton_Click(object sender, System.Windows.RoutedEventArgs e) {
            var window = Window.GetWindow(this) as SettingsWindow;
            if (window != null) {
                window.RestoreBackupButton_Click(sender, e);
            }
        }
    }
}
