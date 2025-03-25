using System.Windows;

namespace Ac.Ratings.Core
{
    public interface IDialogService {
        void ShowMessage(string message, string title, MessageBoxButton buttons);
        bool ShowConfirmation(string message, string title);
        string? ShowOpenFileDialog(string title, string filter, string initialDirectory);
    }
}
