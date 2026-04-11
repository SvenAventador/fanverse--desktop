using System.Globalization;
using System.Windows.Data;

namespace desktop.Converters
{
    public class StatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = value as string;
            return status switch
            {
                "Submitted" => "На рассмотрении",
                "Published" => "Опубликовано",
                "Rejected" => "Отклонено",
                "Draft" => "Черновик",
                _ => status
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}