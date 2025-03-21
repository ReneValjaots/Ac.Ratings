using System.Globalization;
using System.Windows.Data;

namespace Ac.Ratings.Theme.Converters;

public class DoubleToStringConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is int i) return i.ToString();
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}