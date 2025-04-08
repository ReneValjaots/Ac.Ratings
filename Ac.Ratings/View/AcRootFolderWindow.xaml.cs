using System.Windows;
using Ac.Ratings.Core;
using Ac.Ratings.ViewModel;

namespace Ac.Ratings.View {
    /// <summary>
    /// Interaction logic for AcRootFolderWindow.xaml
    /// </summary>
    public partial class AcRootFolderWindow : Window {
        public AcRootFolderViewModel ViewModel { get; }
        public string SelectedPath => ViewModel.SelectedPath;

        public AcRootFolderWindow(AcRootFolderViewModel viewModel) {
            InitializeComponent();
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            DataContext = ViewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            ViewModel.HandleClosing(e);
            if (ViewModel.IsPathValid) {
                DialogResult = true;
            }
        }
    }
}
