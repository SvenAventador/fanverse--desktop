using System.Windows;
using System.Windows.Input;

namespace library.Utils
{
    public partial class RejectionReasonWindow : Window
    {
        public RejectionReasonWindow(string violationType, string reason, DateTime createdAt)
        {
            InitializeComponent();

            Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);

            var moscowTime = TimeZoneInfo.ConvertTimeFromUtc(createdAt, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));

            ViolationTypeText.Text = $"Тип нарушения: {GetViolationTypeText(violationType)}";
            ReasonText.Text = reason;
            DateText.Text = moscowTime.ToString("dd.MM.yyyy HH:mm");
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private static string GetViolationTypeText(string type)
        {
            return type switch
            {
                "Spam" => "Спам",
                "HateSpeech" => "Оскорбления / Ненависть",
                "Copyright" => "Нарушение авторских прав",
                "Explicit" => "Слишком откровенный контент",
                "Other" => "Другое",
                _ => type
            };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
            => Close();
    }
}