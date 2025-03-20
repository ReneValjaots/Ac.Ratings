using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Ac.Ratings.Theme.Converters;

public class LightenColorConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is Color color && parameter is string param && double.TryParse(param, out double factor)) {
            factor = Math.Clamp(factor, 0, 1);
            return Color.FromArgb(
                color.A,
                (byte)Math.Min(255, color.R + (255 - color.R) * factor),
                (byte)Math.Min(255, color.G + (255 - color.G) * factor),
                (byte)Math.Min(255, color.B + (255 - color.B) * factor));
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}