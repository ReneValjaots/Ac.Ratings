using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ac.Ratings.View.Pages {
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl {
        public Home() {
            InitializeComponent();
            DataContext = (Application.Current.MainWindow as MainWindow)?._viewModel;
        }

        private static readonly Regex _regex = new Regex("[^0-9]+");

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e) {
            if (e.DataObject.GetDataPresent(typeof(string))) {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (_regex.IsMatch(text)) {
                    e.CancelCommand();
                }
            }
            else {
                e.CancelCommand();
            }
        }
    }
}
