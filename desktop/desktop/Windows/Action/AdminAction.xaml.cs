using library.Core.Constants;
using library.Core.Interfaces;
using library.Core.Models.Requests;
using library.Core.Models.Responses;
using library.Infrastructure.HttpClient;
using library.Infrastructure.Services;
using library.Utils;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace desktop.Windows.Action
{
    public partial class AdminAction : Window
    {
        private readonly IAdminService _adminService;
        private readonly AdminResponse _editingAdmin;
        private readonly bool _isEditMode;
        private bool _isPasswordVisible = false;

        public AdminAction(AdminResponse admin = null!)
        {
            InitializeComponent();

            var apiClient = new ApiClient(ApiEndpoints.BaseUrl);
            _adminService = new AdminService(apiClient);

            _isEditMode = admin != null;
            _editingAdmin = admin!;

            SetupWindow();
            LoadAdminData();
        }

        private void TogglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;

            if (_isPasswordVisible)
            {
                var password = PasswordBox.Password;
                PasswordVisibleBox.Text = password;

                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordVisibleBox.Visibility = Visibility.Visible;

                var path = (Path)EyeIcon;
                path.Data = Geometry.Parse("M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5zM12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5zm0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3z");
                path.Fill = (System.Windows.Media.Brush)FindResource("PrimaryGradient");
            }
            else
            {
                var text = PasswordVisibleBox.Text;
                PasswordBox.Password = text;

                PasswordVisibleBox.Visibility = Visibility.Collapsed;
                PasswordBox.Visibility = Visibility.Visible;

                var path = (Path)EyeIcon;
                path.Data = Geometry.Parse("M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5zM12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5zm0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3z");
                path.Fill = (Brush)new SolidColorBrush(Color.FromRgb(136, 136, 136));
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                if (!_isEditMode)
                    await CreateAdmin();
                else
                    await UpdateAdmin();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SetupWindow()
        {
            if (_isEditMode)
            {
                TitleText.Text = "Редактирование администратора";
                PasswordLabel.Visibility = Visibility.Collapsed;
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordHint.Visibility = Visibility.Collapsed;
                TogglePasswordButton.Visibility = Visibility.Collapsed;
                GeneratePasswordButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                TitleText.Text = "Добавление администратора";
                PasswordLabel.Visibility = Visibility.Visible;
                PasswordBox.Visibility = Visibility.Visible;
                PasswordHint.Visibility = Visibility.Visible;
                TogglePasswordButton.Visibility = Visibility.Visible;
                GeneratePasswordButton.Visibility = Visibility.Visible;
            }
        }

        private void LoadAdminData()
        {
            if (!_isEditMode ||
                _editingAdmin == null)
                return;

            NicknameBox.Text = _editingAdmin.Nickname;
            EmailBox.Text = _editingAdmin.Email;
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(NicknameBox.Text))
            {
                ShowError("Введите никнейм!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(EmailBox.Text))
            {
                ShowError("Введите email!");
                return false;
            }

            if (!_isEditMode && string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                ShowError("Введите пароль!");
                return false;
            }

            return true;
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;

            var timer = new System.Timers.Timer(3000);
            timer.Elapsed += (s, e) =>
            {
                Dispatcher.Invoke(() => ErrorText.Visibility = Visibility.Collapsed);
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        private async Task CreateAdmin()
        {
            var password = _isPasswordVisible ? PasswordVisibleBox.Text
                                              : PasswordBox.Password;

            var request = new AdminRequest
            {
                Nickname = NicknameBox.Text.Trim(),
                Email = EmailBox.Text.Trim(),
                Password = password,
                RegistrationDate = DateTime.Now
            };

            await _adminService.AddAdminAsync(request);

            MessageBoxWindow.Show("Администратор успешно добавлен!",
                                  "Успех",
                                  MessageBoxIcon.Success,
                                  MessageBoxButtons.OK);
        }

        private async Task UpdateAdmin()
        {
            var request = new AdminRequest
            {
                Id = _editingAdmin.Id,
                Nickname = NicknameBox.Text.Trim(),
                Email = EmailBox.Text.Trim()
            };

            await _adminService.UpdateAdminAsync(request);

            MessageBoxWindow.Show("Администратор успешно обновлен!",
                                  "Успех",
                                  MessageBoxIcon.Success,
                                  MessageBoxButtons.OK);
        }

        private void GeneratePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var password = GenerateRandomPassword();

            if (_isPasswordVisible)
                PasswordVisibleBox.Text = password;
            else
                PasswordBox.Password = password;

            HighlightPasswordField();
        }

        private static string GenerateRandomPassword(int length = 12)
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string special = "!@#$%^&*";

            var random = new Random();
            var password = new char[length];

            password[0] = lower[random.Next(lower.Length)];
            password[1] = upper[random.Next(upper.Length)];
            password[2] = digits[random.Next(digits.Length)];
            password[3] = special[random.Next(special.Length)];

            var allChars = lower + upper + digits + special;
            for (int i = 4; i < length; i++)
                password[i] = allChars[random.Next(allChars.Length)];

            return new string([.. password.OrderBy(x => random.Next())]);
        }

        private void HighlightPasswordField()
        {
            var brush = PasswordBox.Background;
            PasswordBox.Background = new SolidColorBrush(Color.FromRgb(50, 30, 80));

            var timer = new System.Timers.Timer(500);
            timer.Elapsed += (s, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    PasswordBox.Background = (Brush)FindResource("CardGradient");
                });
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }
    }
}
