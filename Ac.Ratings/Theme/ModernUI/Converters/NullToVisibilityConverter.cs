using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ac.Ratings.Theme.ModernUI.Converters {
    /// <summary>
    /// Converts a null value to Visibility.Visible and any other value to Visibility.Collapsed
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var flag = value == null;
            var inverse = parameter as string == "inverse";

            if (inverse) {
                return flag ? Visibility.Collapsed : Visibility.Visible;
            }
            else {
                return flag ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}
