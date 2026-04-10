using library.Core.Constants;
using library.Core.Interfaces;
using library.Infrastructure.HttpClient;
using library.Infrastructure.Services;
using library.Infrastructure.Storage;
using library.Utils;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;

namespace desktop.Windows.Auth
{
    public partial class Login : Window
    {
        private readonly IAuthService _authService;
        private CancellationTokenSource? _cts;

        public Login()
        {
            InitializeComponent();

            var apiClient = new ApiClient(ApiEndpoints.BaseUrl);
            _authService = new AuthService(apiClient);

            LoadSavedCredentials();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            await ExecuteLoginAsync(_cts.Token);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
            Application.Current.Shutdown();
        }

        protected override void OnClosed(EventArgs e)
        {
            _cts?.Dispose();
            base.OnClosed(e);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void LoadSavedCredentials()
        {
            var (email, remember) = SecureStorage.LoadCredentials();

            if (remember && 
                !string.IsNullOrEmpty(email))
            {
                EmailBox.Text = email;
                RememberCheckBox.IsChecked = true;
                PasswordBox.Focus();
            }
        }

        private void SetLoading(bool isLoading)
        {
            Dispatcher.Invoke(() =>
            {
                LoginButton.IsEnabled = !isLoading;
                EmailBox.IsEnabled = !isLoading;
                PasswordBox.IsEnabled = !isLoading;
                RememberCheckBox.IsEnabled = !isLoading;

                LoadingSpinner.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
                LoginButton.Content = isLoading ? "" : "ВОЙТИ";
            });
        }

        private void ShowError(string message)
        {
            Dispatcher.Invoke(() =>
            {
                ErrorTextBlock.Text = message;
                ErrorTextBlock.Visibility = Visibility.Visible;

                var timer = new System.Timers.Timer(4000);
                timer.Elapsed += (s, e) =>
                {
                    Dispatcher.Invoke(() => ErrorTextBlock.Visibility = Visibility.Collapsed);
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            });
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(EmailBox.Text))
            {
                ShowError("Введите email");
                return false;
            }

            if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                ShowError("Введите пароль");
                return false;
            }

            return true;
        }

        private async Task ExecuteLoginAsync(CancellationToken cancellationToken)
        {
            SetLoading(true);

            try
            {
                var token = await _authService.LoginAsync(
                    EmailBox.Text.Trim(),
                    PasswordBox.Password,
                    cancellationToken
                );

                if (!string.IsNullOrEmpty(token))
                {
                    SecureStorage.SaveCredentials(EmailBox.Text, RememberCheckBox.IsChecked == true);

                    MessageBoxWindow.Show("Вход выполнен успешно!",
                                          "Успех",
                                          MessageBoxIcon.Success,
                                          MessageBoxButtons.OK);

                    new AdminDashboard().Show();    

                    Close();
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (UnauthorizedAccessException ex)
            {
                ShowError(ex.Message);
            }
            catch (HttpRequestException ex)
            {
                ShowError($"Ошибка подключения: {ex.Message}");
            }
            catch (Exception ex)
            {
                ShowError($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }
    }
}
