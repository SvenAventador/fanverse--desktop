using System.Globalization;
using System.Windows.Data;

namespace desktop.Converters
{
    public class ModerationStatusConverter : IValueConverter
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
                _ => status ?? "Неизвестно"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = value as string;

            return status switch
            {
                "На рассмотрении" => "Submitted",
                "Опубликовано" => "Published",
                "Отклонено" => "Rejected",
                "Черновик" => "Draft",
                _ => status
            };
        }
    }

}
