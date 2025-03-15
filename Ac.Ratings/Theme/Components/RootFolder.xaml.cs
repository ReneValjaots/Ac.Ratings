using System.Windows;
using System.Windows.Controls;

namespace Ac.Ratings.Theme.Components
{
    /// <summary>
    /// Interaction logic for RootFolder.xaml
    /// </summary>
    public partial class RootFolder : UserControl
    {
        public RootFolder()
        {
            InitializeComponent();
        }

        private void ResetRootFolder_Click(object sender, System.Windows.RoutedEventArgs e) {
            var window = Window.GetWindow(this) as SettingsWindow;
            if (window != null) {
                window.ResetRootFolder_Click(sender, e);
            }
        }
    }
}
