using System.Windows;
using System.Windows.Controls;

namespace Ac.Ratings.Theme.Components
{
    /// <summary>
    /// Interaction logic for General.xaml
    /// </summary>
    public partial class General : UserControl
    {
        public General()
        {
            InitializeComponent();
        }

        private void ResetRatingsButton_Click(object sender, System.Windows.RoutedEventArgs e) {
            var window = Window.GetWindow(this) as SettingsWindow;
            if (window != null) {
                window.ResetRatingsButton_Click(sender, e);
            }
        }

        private void ResetExtraFeatures_Click(object sender, System.Windows.RoutedEventArgs e) {
            var window = Window.GetWindow(this) as SettingsWindow;
            if (window != null) {
                window.ResetExtraFeatures_Click(sender, e);
            }
        }

        private void TransferRatingsButton_OnClick(object sender, System.Windows.RoutedEventArgs e) {
            var window = Window.GetWindow(this) as SettingsWindow;
            if (window != null) {
                window.TransferRatingsButton_OnClick(sender, e);
            }
        }
    }
}
