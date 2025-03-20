using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Ac.Ratings.Theme.Converters;

public class DarkenColorConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is Color color && parameter is string param && double.TryParse(param, out double factor)) {
            factor = Math.Clamp(factor, 0, 1);
            return Color.FromArgb(
                color.A,
                (byte)(color.R * (1 - factor)),
                (byte)(color.G * (1 - factor)),
                (byte)(color.B * (1 - factor)));
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}