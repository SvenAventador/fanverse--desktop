using System.Globalization;
using System.Windows.Data;

namespace desktop.Converters
{
    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = value as string;
            return status switch
            {
                "Submitted" => "#FF9800",
                "Published" => "#4CAF50",
                "Rejected" => "#FF5252",
                "Draft" => "#888",
                _ => "#888"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}