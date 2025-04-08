using Ac.Ratings.Core;
using System.IO;
using System.Windows;

namespace Ac.Ratings.ViewModel {
    public class AcRootFolderViewModel : Core.ViewModel {
        private IDialogService _dialogService;
        private string _rootFolderPath = string.Empty;

        public string RootFolderPath {
            get => _rootFolderPath;
            set => SetField(ref _rootFolderPath, value);
        }

        public string SelectedPath { get; private set; } = string.Empty;
        public bool IsPathValid { get; private set; }

        public RelayCommand OkCommand { get; }

        public AcRootFolderViewModel() {
            OkCommand = new RelayCommand(ExecuteOk);
        }

        public void SetDialogService(IDialogService dialogService) {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        private void ExecuteOk() {
            if (string.IsNullOrWhiteSpace(RootFolderPath)) {
                _dialogService.ShowMessage("Please enter a path.", "Invalid Input", MessageBoxButton.OK);
                return;
            }
            if (Directory.Exists(RootFolderPath)) {
                var carsPath = Path.Combine(RootFolderPath, "content", "cars");

                if (!Directory.Exists(carsPath)) {
                    _dialogService.ShowMessage(
                        "The selected Assetto Corsa root folder does not meet the required folder structure.\n" +
                        "Ensure that the root folder contains a 'content' subfolder with a 'cars' directory inside it.\n",
                        "Invalid Folder Structure", MessageBoxButton.OK);
                    return;
                }

                SelectedPath = carsPath;
                IsPathValid = true;
                _dialogService.ShowMessage(
                    "Root folder selected successfully!",
                    "Success",
                    MessageBoxButton.OK);
            }
            else {
                _dialogService.ShowMessage("The provided path does not exist. Please enter a valid path.", "Invalid Path", MessageBoxButton.OK);
            }
        }


        public void HandleClosing(System.ComponentModel.CancelEventArgs e) {
            if (!IsPathValid) {
                bool shouldExit = _dialogService.ShowConfirmation(
                    "No valid root folder has been selected. Closing this window will exit the application. Are you sure?",
                    "Confirm Exit");
                if (!shouldExit) {
                    e.Cancel = true;
                }
                else {
                    Environment.Exit(0);
                }
            }
        }
    }
}
