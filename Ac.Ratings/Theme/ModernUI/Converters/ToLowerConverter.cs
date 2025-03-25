using System.Diagnostics.CodeAnalysis;
using System.Windows.Data;

namespace Ac.Ratings.Theme.ModernUI.Converters {
    /// <summary>
    /// Converts string values to lower case.
    /// </summary>
    public class ToLowerConverter : IValueConverter {

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture) {
            var strValue = value?.ToString();

            return strValue?.ToLowerInvariant();
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}
