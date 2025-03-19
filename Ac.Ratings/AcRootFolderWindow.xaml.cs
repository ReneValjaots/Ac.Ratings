using System.IO;
using System.Windows;
using Ac.Ratings.Theme.ModernUI.Controls;

namespace Ac.Ratings {
    /// <summary>
    /// Interaction logic for AcRootFolderWindow.xaml
    /// </summary>
    public partial class AcRootFolderWindow : ModernWindowBase {
        public string SelectedPath { get; private set; } = string.Empty;
        private bool _isCanceling = false;

        public AcRootFolderWindow() {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e) {
            if (Directory.Exists(RootFolderPath.Text)) {
                var rootPath = RootFolderPath.Text;
                var carsPath = Path.Combine(rootPath, "content", "cars");

                if (!Directory.Exists(carsPath)) {
                    ModernDialog.ShowMessage(
                        "The selected Assetto Corsa root folder does not meet the required folder structure.\n" +
                        "Ensure that the root folder contains a 'content' subfolder with a 'cars' directory inside it.\n",
                        "Invalid Folder Structure", MessageBoxButton.OK);
                    return;
                }

                SelectedPath = carsPath;
                DialogResult = true;
            }
            else {
                ModernDialog.ShowMessage("The provided path does not exist. Please enter a valid path.", "Invalid path", MessageBoxButton.OK);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            _isCanceling = true; 
            Close(); 
        }

        private bool ConfirmExit() {
            var result = ModernDialog.ShowMessage(
                "Exiting this window without selecting a valid root folder will close the application. Are you sure?",
                "Confirm Exit", MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (_isCanceling) {
                if (!ConfirmExit()) {
                    e.Cancel = true;
                    _isCanceling = false;
                    return;
                }

                _isCanceling = false;
            }
            else if (!DialogResult.HasValue) {
                if (!ConfirmExit()) {
                    e.Cancel = true;
                    return;
                }
            }

            if (!DialogResult.HasValue) {
                Environment.Exit(0);
            }
        }
    }
}
