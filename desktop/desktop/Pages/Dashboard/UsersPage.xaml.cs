using library.Core.Constants;
using library.Core.Interfaces;
using library.Core.Models.Requests;
using library.Core.Models.Responses;
using library.Entities;
using library.Infrastructure.HttpClient;
using library.Infrastructure.Services;
using library.Utils;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace desktop.Pages.Dashboard
{
    public partial class UsersPage : Page
    {
        private readonly IModerationService _moderationService;
        private ObservableCollection<UserResponse> _allUsers;
        private ObservableCollection<UserResponse> _filteredUsers;

        private UserResponse? _selectedUser;

        public UsersPage()
        {
            InitializeComponent();

            var apiClient = new ApiClient(ApiEndpoints.BaseUrl);
            _moderationService = new ModerationService(apiClient);

            _allUsers = [];
            _filteredUsers = [];

            UsersItemsControl.ItemsSource = _filteredUsers;

            Loaded += async (s, e) => await LoadUsers();
        }

        private async Task LoadUsers()
        {
            try
            {
                ShowLoading(true);

                var users = await _moderationService.GetAllUsersAsync();

                _allUsers.Clear();
                foreach (var user in users.OrderBy(x => x.Id))
                {
                    user.HasBanReason = !string.IsNullOrEmpty(user.BanReason);
                    user.AvatarImage = new BitmapImage(new Uri("pack://application:,,,/Assets/static.jpg"));
                    _allUsers.Add(user);
                }

                ApplyFilter();

                foreach (var user in _allUsers.Where(u => !string.IsNullOrEmpty(u.AvatarUrl)))
                    await LoadAvatarForUser(user);
            }
            catch (Exception ex)
            {
                MessageBoxWindow.Show($"Ошибка загрузки пользователей: {ex.Message}",
                                       "Ошибка",
                                       MessageBoxIcon.Error,
                                       MessageBoxButtons.OK);
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private async Task LoadAvatarForUser(UserResponse user)
        {
            await Dispatcher.InvokeAsync(async () =>
            {
                if (string.IsNullOrEmpty(user.AvatarUrl))
                {
                    user.AvatarImage = new BitmapImage(new Uri("pack://application:,,,/Assets/static.jpg", UriKind.Relative));
                    return;
                }

                try
                {
                    var fullUrl = (ApiEndpoints.BaseUrl + user.AvatarUrl).Replace("api", "");

                    using var httpClient = new HttpClient();
                    var imageBytes = await httpClient.GetByteArrayAsync(fullUrl);

                    using var stream = new MemoryStream(imageBytes);
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    user.AvatarImage = bitmap;
                }
                catch
                {
                    user.AvatarImage = new BitmapImage(new Uri("pack://application:,,,/Assets/static.jpg", UriKind.Relative));
                }
            });
        }

        private T FindVisualChild<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T element && element.Name == name)
                    return element;

                var result = FindVisualChild<T>(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }



        private void ApplyFilter()
        {
            var searchText = SearchBox?.Text?.Trim().ToLower() ?? "";

            _filteredUsers.Clear();

            var filtered = string.IsNullOrEmpty(searchText)
                ? _allUsers
                : _allUsers.Where(u =>
                        u.Nickname?.ToLower(System.Globalization.CultureInfo.CurrentCulture).Contains(searchText,
                            StringComparison.CurrentCultureIgnoreCase) == true ||
                        u.Email?.ToLower(System.Globalization.CultureInfo.CurrentCulture).Contains(searchText,
                            StringComparison.CurrentCultureIgnoreCase) == true ||
                        u.Id.ToString().Contains(searchText));

            foreach (var user in filtered.OrderBy(x => x.Id))
                _filteredUsers.Add(user);
                
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyFilter();

        private void ShowBanReason_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.Tag is UserResponse user &&
                !string.IsNullOrEmpty(user.BanReason))
            {
                BanReasonText.Text = user.BanReason;
                BanReasonDialog.Visibility = Visibility.Visible;
            }
        }

        private async void ToggleBan_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.Tag is not UserResponse user)
                return;

            _selectedUser = user;

            if (user.IsBanned)
            {
                var result = MessageBoxWindow.Show($"Разблокировать пользователя {user.Nickname}?",
                                                    "Подтверждение",
                                                    MessageBoxIcon.Question,
                                                    MessageBoxButtons.YesNo);

                if (result)
                    await UnbanUser();
            }
            else
            {
                BanReasonInputBox.Text = "";
                BanDialog.Visibility = Visibility.Visible;
            }
        }

        private async Task UnbanUser()
        {
            try
            {
                ShowLoading(true);

                var request = new BanRequest
                {
                    Id = _selectedUser.Id
                };

                var result = await _moderationService.UnbanUserAsync(request);

                if (result != null)
                {
                    await LoadUsers();
                    MessageBoxWindow.Show("Пользователь разблокирован!",
                                          "Успех",
                                          MessageBoxIcon.Success,
                                          MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBoxWindow.Show($"Ошибка: {ex.Message}",
                                       "Ошибка",
                                       MessageBoxIcon.Error,
                                       MessageBoxButtons.OK);
            }
            finally
            {
                ShowLoading(false);
                BanDialog.Visibility = Visibility.Collapsed;
            }
        }

        private async void ConfirmBan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BanReasonInputBox.Text))
            {
                MessageBoxWindow.Show("Укажите причину блокировки!",
                                      "Внимание",
                                      MessageBoxIcon.Info,
                                      MessageBoxButtons.OK);
                return;
            }

            try
            {
                ShowLoading(true);

                var request = new BanRequest
                {
                    Id = _selectedUser.Id,
                    BanReason = BanReasonInputBox.Text.Trim()
                };

                var result = await _moderationService.BanUserAsync(request);

                if (result != null)
                {
                    await LoadUsers();
                    MessageBoxWindow.Show("Пользователь заблокирован!",
                                          "Успех",
                                          MessageBoxIcon.Success,
                                          MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBoxWindow.Show($"Ошибка: {ex.Message}",
                                       "Ошибка",
                                       MessageBoxIcon.Error,
                                       MessageBoxButtons.OK);
            }
            finally
            {
                ShowLoading(false);
                BanDialog.Visibility = Visibility.Collapsed;
            }
        }

        private void CancelBan_Click(object sender, RoutedEventArgs e)
            => BanDialog.Visibility = Visibility.Collapsed;

        private void CloseBanReasonDialog_Click(object sender, RoutedEventArgs e)
            => BanReasonDialog.Visibility = Visibility.Collapsed;

        private void ShowLoading(bool isLoading)
        {
            Dispatcher.Invoke(() =>
            {
                LoadingPanel.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
                UsersItemsControl.Visibility = isLoading ? Visibility.Collapsed : Visibility.Visible;
            });
        }
    }
}