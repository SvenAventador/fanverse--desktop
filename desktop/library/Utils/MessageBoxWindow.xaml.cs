using System.Windows;
using System.Windows.Controls;

namespace library.Utils
{
    public enum MessageBoxIcon
    {
        None,
        Success,
        Question,
        Error,
        Info
    }

    public enum MessageBoxButtons
    {
        OK,
        YesNo
    }

    public partial class MessageBoxWindow : Window
    {
        private bool _result = false;

        public MessageBoxWindow(string title, 
                                string message, 
                                MessageBoxIcon icon,
                                MessageBoxButtons buttons)
        {
            InitializeComponent();

            TitleText.Text = title;
            MessageText.Text = message;
            SetIcon(icon);
            SetButtons(buttons);
        }

        private void SetIcon(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Success:
                    IconContent.Content = FindResource("SuccessIcon");
                    break;
                case MessageBoxIcon.Question:
                    IconContent.Content = FindResource("QuestionIcon");
                    break;
                case MessageBoxIcon.Error:
                    IconContent.Content = FindResource("ErrorIcon");
                    break;
                case MessageBoxIcon.Info:
                    IconContent.Content = FindResource("InfoIcon");
                    break;
                case MessageBoxIcon.None:
                    break;
                default:
                    IconContent.Content = null;
                    break;
            }
        }

        private void SetButtons(MessageBoxButtons buttons)
        {
            ButtonsPanel.Children.Clear();

            if (buttons == MessageBoxButtons.OK)
            {
                var okButton = new Button
                {
                    Content = "OK",
                    Style = (Style)FindResource("OkButtonStyle")
                };
                okButton.Click += (s, e) =>
                {
                    _result = true;
                    Close();
                };
                ButtonsPanel.Children.Add(okButton);
            }
            else if (buttons == MessageBoxButtons.YesNo)
            {
                var yesButton = new Button
                {
                    Content = "Да",
                    Style = (Style)FindResource("YesButtonStyle")
                };
                yesButton.Click += (s, e) =>
                {
                    _result = true;
                    Close();
                };
                ButtonsPanel.Children.Add(yesButton);

                var noButton = new Button
                {
                    Content = "Нет",
                    Style = (Style)FindResource("CancelButtonStyle"),
                    Margin = new Thickness(10, 0, 0, 0)
                };
                noButton.Click += (s, e) =>
                {
                    _result = false;
                    Close();
                };
                ButtonsPanel.Children.Add(noButton);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _result = false;
            Close();
        }

        public static bool Show(string message, 
                                string title = "FanVerse",
                                MessageBoxIcon icon = MessageBoxIcon.None,
                                MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            var window = new MessageBoxWindow(title, message, icon, buttons)
            {
                Owner = Application.Current
                                   .Windows
                                   .OfType<Window>()
                                   .FirstOrDefault(w => w.IsActive)
            };

            window.ShowDialog();
            return window._result;
        }
    }
}