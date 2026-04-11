using library.Core.Constants;
using library.Core.Interfaces;
using library.Infrastructure.HttpClient;
using library.Infrastructure.Services;
using library.Utils;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace desktop.Windows.Action
{
    public partial class ForgotPasswordWindow : Window
    {
        private readonly IUserService _userService;
        private string _resetCode;

        public ForgotPasswordWindow()
        {
            InitializeComponent();

            var apiClient = new ApiClient(ApiEndpoints.BaseUrl);
            _userService = new UserService(apiClient);

            UpdateStep(1);
        }

        private void UpdateStep(int step)
        {
            // Сброс ошибок
            EmailError.Visibility = Visibility.Collapsed;
            CodeError.Visibility = Visibility.Collapsed;
            PasswordError.Visibility = Visibility.Collapsed;
            ConfirmError.Visibility = Visibility.Collapsed;

            // Обновляем индикаторы
            var steps = new[] {
                (Step1Indicator, Step1Text, 1),
                (Step2Indicator, Step2Text, 2),
                (Step3Indicator, Step3Text, 3)
            };

            for (int i = 0; i < steps.Length; i++)
            {
                var (indicator, text, num) = steps[i];
                var isActive = i + 1 == step;
                var isCompleted = i + 1 < step;

                if (isCompleted)
                {
                    indicator.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
                    indicator.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
                    text.Text = "✓";
                    text.Foreground = Brushes.White;
                }
                else if (isActive)
                {
                    indicator.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8A2BE2"));
                    indicator.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8A2BE2"));
                    text.Text = num.ToString();
                    text.Foreground = Brushes.White;
                }
                else
                {
                    indicator.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A2438"));
                    indicator.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A2438"));
                    text.Text = num.ToString();
                    text.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888"));
                }
            }

            // Показываем нужную панель
            Step1Panel.Visibility = step == 1 ? Visibility.Visible : Visibility.Collapsed;
            Step2Panel.Visibility = step == 2 ? Visibility.Visible : Visibility.Collapsed;
            Step3Panel.Visibility = step == 3 ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void SendCodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailBox.Text))
            {
                EmailError.Text = "Введите email";
                EmailError.Visibility = Visibility.Visible;
                return;
            }

            SetLoading(true);

            try
            {
                var code = await _userService.ForgotPasswordAsync(EmailBox.Text.Trim());

                if (!string.IsNullOrEmpty(code))
                {
                    _resetCode = code;
                    UpdateStep(2);
                }
            }
            catch (Exception ex)
            {
                EmailError.Text = ex.Message;
                EmailError.Visibility = Visibility.Visible;
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void VerifyCodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CodeBox.Text))
            {
                CodeError.Text = "Введите код подтверждения";
                CodeError.Visibility = Visibility.Visible;
                return;
            }

            if (CodeBox.Text.Trim() != _resetCode)
            {
                CodeError.Text = "Неверный код подтверждения";
                CodeError.Visibility = Visibility.Visible;
                return;
            }

            UpdateStep(3);
        }

        private async void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var newPassword = NewPasswordBox.Password;
            var confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                PasswordError.Text = "Введите новый пароль";
                PasswordError.Visibility = Visibility.Visible;
                return;
            }

            if (newPassword != confirmPassword)
            {
                ConfirmError.Text = "Пароли не совпадают";
                ConfirmError.Visibility = Visibility.Visible;
                return;
            }

            if (newPassword.Length < 9)
            {
                PasswordError.Text = "Пароль должен содержать минимум 9 символов";
                PasswordError.Visibility = Visibility.Visible;
                return;
            }

            SetLoading(true);

            try
            {
                var response = await _userService.ResetPasswordAsync(EmailBox.Text.Trim(), newPassword);

                if (response != null)
                {
                    MessageBoxWindow.Show("Пароль успешно изменён! Войдите с новым паролем.",
                        "Успех", MessageBoxIcon.Success, MessageBoxButtons.OK);
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                PasswordError.Text = ex.Message;
                PasswordError.Visibility = Visibility.Visible;
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void BackToEmailButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateStep(1);
            CodeBox.Text = "";
        }

        private void BackToCodeButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateStep(2);
            NewPasswordBox.Password = "";
            ConfirmPasswordBox.Password = "";
        }

        private void SetLoading(bool isLoading)
        {
            Dispatcher.Invoke(() =>
            {
                SendCodeButton.IsEnabled = !isLoading;
                VerifyCodeButton.IsEnabled = !isLoading;
                ResetPasswordButton.IsEnabled = !isLoading;
                LoadingSpinner.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}