using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ac.Ratings.Core;

namespace Ac.Ratings.Theme.ModernUI.Controls
{
    public class ModernDialog : Window {
        public static readonly DependencyProperty BackgroundContentProperty = DependencyProperty.Register(nameof(BackgroundContent), typeof(object), typeof(ModernDialog));
        public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register(nameof(Buttons), typeof(IEnumerable<Button>), typeof(ModernDialog));

        private readonly RelayCommand<MessageBoxResult> _closeCommand; 
        private Button _okButton;
        private Button _cancelButton;
        private Button _yesButton;
        private Button _noButton;
        private Button _closeButton;
        private MessageBoxResult _messageBoxResult = MessageBoxResult.None;

        public ModernDialog()
        {
            DefaultStyleKey = typeof(ModernDialog);
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            _closeCommand = new RelayCommand<MessageBoxResult>(result => {
                _messageBoxResult = result;

                DialogResult = result switch {
                    MessageBoxResult.OK or MessageBoxResult.Yes => true,
                    MessageBoxResult.Cancel or MessageBoxResult.No => false,
                    _ => null
                };

                Close();
            });

            Buttons = new Button[] { CloseButton };

            // set the default owner to the app main window (if possible)
            if (Application.Current != null && Application.Current.MainWindow != this) {
                Owner = Application.Current.MainWindow;
            }
        }

        private Button CreateCloseDialogButton(string content, bool isDefault, bool isCancel, MessageBoxResult result)
        {
            return new Button {
                Content = content,
                Command = CloseCommand,
                CommandParameter = result,
                IsDefault = isDefault,
                IsCancel = isCancel,
                MinHeight = 21,
                MinWidth = 65,
                Margin = new Thickness(4, 0, 0, 0)
            };
        }

        public ICommand CloseCommand => _closeCommand;

        public Button OkButton
        {
            get
            {
                if (_okButton == null) {
                    _okButton = CreateCloseDialogButton("Ok", true, false, MessageBoxResult.OK);
                }
                return _okButton;
            }
        }

        public Button CancelButton
        {
            get
            {
                if (_cancelButton == null) {
                    _cancelButton = CreateCloseDialogButton("Cancel", false, true, MessageBoxResult.Cancel);
                }
                return _cancelButton;
            }
        }

        public Button YesButton
        {
            get
            {
                if (_yesButton == null) {
                    _yesButton = CreateCloseDialogButton("Yes", true, false, MessageBoxResult.Yes);
                }
                return _yesButton;
            }
        }

        public Button NoButton
        {
            get
            {
                if (_noButton == null) {
                    _noButton = CreateCloseDialogButton("No", false, true, MessageBoxResult.No);
                }
                return _noButton;
            }
        }

        public Button CloseButton
        {
            get
            {
                if (_closeButton == null) {
                    _closeButton = CreateCloseDialogButton("Close", true, false, MessageBoxResult.None);
                }
                return _closeButton;
            }
        }

        public object BackgroundContent
        {
            get => GetValue(BackgroundContentProperty);
            set => SetValue(BackgroundContentProperty, value);
        }

        public IEnumerable<Button> Buttons
        {
            get => (IEnumerable<Button>)GetValue(ButtonsProperty);
            set => SetValue(ButtonsProperty, value);
        }

        public MessageBoxResult MessageBoxResult => _messageBoxResult;

        public static MessageBoxResult ShowMessage(string text, string title, MessageBoxButton button, Window owner = null)
        {
            var dlg = new ModernDialog {
                Title = title,
                Content = new TextBlock {
                    Text = text,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 8)
                },
                MinHeight = 0,
                MinWidth = 0,
                MaxHeight = 480,
                MaxWidth = 640,
            };
            if (owner != null) {
                dlg.Owner = owner;
            }

            dlg.Buttons = GetButtons(dlg, button);
            dlg.ShowDialog();
            return dlg._messageBoxResult;
        }

        private static IEnumerable<Button> GetButtons(ModernDialog owner, MessageBoxButton button)
        {
            if (button == MessageBoxButton.OK) {
                yield return owner.OkButton;
            }
            else if (button == MessageBoxButton.OKCancel) {
                yield return owner.OkButton;
                yield return owner.CancelButton;
            }
            else if (button == MessageBoxButton.YesNo) {
                yield return owner.YesButton;
                yield return owner.NoButton;
            }
            else if (button == MessageBoxButton.YesNoCancel) {
                yield return owner.YesButton;
                yield return owner.NoButton;
                yield return owner.CancelButton;
            }
        }
    }
}