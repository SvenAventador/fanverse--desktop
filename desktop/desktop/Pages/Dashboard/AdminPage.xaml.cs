using library.Core.Constants;
using library.Core.Interfaces;
using library.Core.Models.Responses;
using library.Infrastructure.HttpClient;
using library.Infrastructure.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace desktop.Pages.Dashboard
{
    public partial class AdminPage : Page
    {
        private readonly IAdminService _adminService;
        private readonly IApiClient _apiClient;
        private ObservableCollection<AdminResponse> _allAdmins;

        public AdminPage()
        {
            InitializeComponent();

            var apiClient = new ApiClient(ApiEndpoints.BaseUrl);
            _apiClient = apiClient;
            _adminService = new AdminService(apiClient);

            _allAdmins = [];

            LoadAdminsData();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyFilter();

        private void AddAdminButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditAdmin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteAdmin_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void LoadAdminsData()
        {
            try
            {
                var adminsData = await _adminService.GetAllAdminsAsync();

                Dispatcher.Invoke(() =>
                {
                    _allAdmins.Clear();
                    foreach (var admin in adminsData.OrderBy(x => x.Id))
                        _allAdmins.Add(admin);

                    AdminsDataGrid.ItemsSource = _allAdmins;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void ApplyFilter()
        {
            var searchText = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                AdminsDataGrid.ItemsSource = _allAdmins;
                return;
            }

            var filtered = _allAdmins.Where(admin =>
                (admin.Nickname?.ToLower().Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true) ||
                (admin.Email?.ToLower().Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true)
            ).ToList();

            AdminsDataGrid.ItemsSource = filtered;
        }
    }
}
