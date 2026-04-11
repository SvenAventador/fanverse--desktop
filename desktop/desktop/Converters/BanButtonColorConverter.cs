using System.Globalization;
using System.Windows.Data;

namespace desktop.Converters
{
    public class BanButtonColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value ? "#4CAF50" : "#FF5252";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}