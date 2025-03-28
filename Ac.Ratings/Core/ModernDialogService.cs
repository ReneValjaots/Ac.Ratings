using System.Windows;
using Ac.Ratings.Theme.ModernUI.Controls;

namespace Ac.Ratings.Core {
    public class ModernDialogService : IDialogService {
        private readonly Window _owner;

        public ModernDialogService(Window owner) {
            _owner = owner;
        }

        public void ShowMessage(string message, string title, MessageBoxButton buttons) {
            ModernDialog.ShowMessage(message, title, buttons, _owner);
        }

        public bool ShowConfirmation(string message, string title) {
            return ModernDialog.ShowMessage(message, title, MessageBoxButton.YesNo, _owner) == MessageBoxResult.Yes;
        }

        public string? ShowOpenFileDialog(string title, string filter, string initialDirectory) {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog {
                Title = title,
                Filter = filter,
                InitialDirectory = initialDirectory
            };
            return openFileDialog.ShowDialog(_owner) == true ? openFileDialog.FileName : null;
        }
    }
}