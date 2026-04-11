using System.Globalization;
using System.Windows.Data;

namespace desktop.Converters
{
    public class BanStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value ? "#FF5252" : "#4CAF50";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}