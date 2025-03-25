using System.Windows.Data;

namespace Ac.Ratings.Theme.ModernUI.Converters {
    /// <summary>
    /// Converts string values to upper case.
    /// </summary>
    public class ToUpperConverter : IValueConverter {
        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture) {
            var strValue = value?.ToString();

            return strValue?.ToUpperInvariant();
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}
