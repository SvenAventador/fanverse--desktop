using desktop.Pages.Dashboard;
using desktop.Windows.Auth;
using library.Core.Constants;
using library.Core.Interfaces;
using library.Infrastructure.HttpClient;
using library.Infrastructure.Services;
using library.Infrastructure.Storage;
using Microsoft.Win32;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace desktop
{
    public partial class AdminDashboard : Window
    {
        private readonly IAuthService _authService;
        private readonly IAdminService _adminService;
        private readonly IApiClient _apiClient;

        public AdminDashboard()
        {
            InitializeComponent();

            var apiClient = new ApiClient(ApiEndpoints.BaseUrl);
            _apiClient = apiClient;
            _authService = new AuthService(apiClient);
            _adminService = new AdminService(apiClient);

            SetActiveButton(NavAdminButton);
            ContentFrame.Navigate(new AdminPage());

            LoadAdminData();

            AvatarBorder.MouseEnter += AvatarBorder_MouseEnter;
            AvatarBorder.MouseLeave += AvatarBorder_MouseLeave;
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var tag = button?.Tag?.ToString();

            SetActiveButton(button!);

            switch (tag)
            {
                case "Администрация":
                    PageTitleText.Text = "Администрация";
                    PageSubtitleText.Text = "Управление администраторами и модераторами";
                    ContentFrame.Navigate(new AdminPage());
                    break;

                case "Статистика":
                    PageTitleText.Text = "Статистика";
                    PageSubtitleText.Text = "Аналитика платформы";
                    ContentFrame.Navigate(new StatisticsPage());
                    break;

                case "Книги":
                    PageTitleText.Text = "Книги";
                    PageSubtitleText.Text = "Управление книгами и главами";
                    ContentFrame.Navigate(new BooksPage());
                    break;

                case "Пользователи":
                    PageTitleText.Text = "Пользователи";
                    PageSubtitleText.Text = "Управление пользователями";
                    ContentFrame.Navigate(new UsersPage());
                    break;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?",
                                         "Выход",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _authService?.Logout();

                new Login().Show();
                Close();
            }
        }

        private async void AvatarBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Выберите аватар",
                Filter = "Изображения (*.png;*.jpg;*.jpeg;*.gif)|*.png;*.jpg;*.jpeg;*.gif",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == true)
                await UploadNewAvatar(openFileDialog.FileName);
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void CloseButton_Click(object sender, RoutedEventArgs e)
            => Application.Current.Shutdown();

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void AvatarBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            CameraOverlay.Visibility = Visibility.Visible;
            AvatarBorder.Cursor = Cursors.Hand;
        }

        private void AvatarBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            CameraOverlay.Visibility = Visibility.Collapsed;
            AvatarBorder.Cursor = null;
        }

        private void SetActiveButton(Button activeButton)
        {
            var buttons = new[] {
                NavAdminButton,
                NavStatsButton,
                NavBooksButton,
                NavUsersButton
            };

            foreach (var btn in buttons)
            {
                btn.Style = (Style)FindResource(btn == activeButton ? "ActiveNavButtonStyle"
                                                                    : "NavButtonStyle");
            }
        }

        private async void LoadAdminData()
        {
            try
            {
                var adminData = await _adminService.GetCurrentAdminAsync();

                Dispatcher.Invoke(() =>
                {
                    AdminNameText.Text = adminData?.Nickname ?? adminData?.Email ?? "Администратор";

                    if (!string.IsNullOrEmpty(adminData?.AvatarUrl))
                        LoadAvatar(adminData.AvatarUrl);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private async void LoadAvatar(string avatarUrl)
        {
            try
            {
                using var httpClient = new HttpClient();
                var imageBytes = await httpClient.GetByteArrayAsync(
                    (ApiEndpoints.BaseUrl + avatarUrl).Replace("api", "")
                );

                Dispatcher.Invoke(() =>
                {
                    using var stream = new MemoryStream(imageBytes);
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    AvatarImageBrush.ImageSource = bitmap;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки аватара: {ex.Message}");
            }
        }

        private async Task UploadNewAvatar(string filePath)
        {
            try
            {
                SetLoading(true);

                var response = await _adminService.UploadAvatarAsync(filePath, "avatarUrl");

                if (response?.AvatarUrl != null)
                {
                    LoadAvatar(response.AvatarUrl);

                    if (!string.IsNullOrEmpty(response.Token))
                        SecureStorage.SaveToken(response.Token);

                    MessageBox.Show("Аватар успешно обновлён!",
                                    "Успех",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке аватара: {ex.Message}",
                                 "Ошибка",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Error);
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void SetLoading(bool isLoading)
        {
            Dispatcher.Invoke(() =>
            {
                if (isLoading)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    IsEnabled = false;
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    IsEnabled = true;
                }
            });
        }
    }
}