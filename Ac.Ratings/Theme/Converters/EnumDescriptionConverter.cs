using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Ac.Ratings.Theme.Converters;

public class EnumDescriptionConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is Enum enumValue) {
            DescriptionAttribute attribute = enumValue.GetType()
                .GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault() as DescriptionAttribute;
            return attribute?.Description ?? enumValue.ToString();
        }

        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotSupportedException("ConvertBack is not supported for EnumDescriptionConverter.");
    }
}