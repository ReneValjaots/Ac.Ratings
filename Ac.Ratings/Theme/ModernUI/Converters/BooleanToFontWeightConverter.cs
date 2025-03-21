using System.Windows;
using System.Windows.Data;

namespace Ac.Ratings.Theme.ModernUI.Converters {
    /// <summary>
    /// Converts a boolean value to a font weight (false: normal, true: bold)
    /// </summary>
    public class BooleanToFontWeightConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            bool inverse = parameter as string == "inverse";

            var bold = value as bool?;
            if (bold.HasValue && bold.Value) {
                return inverse ? FontWeights.Normal : FontWeights.Bold;
            }

            return inverse ? FontWeights.Bold : FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}
