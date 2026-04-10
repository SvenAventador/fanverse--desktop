using library.Core.Constants;
using library.Core.Interfaces;
using library.Core.Models.Common;
using library.Infrastructure.HttpClient;
using library.Infrastructure.Services;
using library.Utils;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;

namespace desktop.Pages.Dashboard
{
    public partial class StatisticsPage : Page
    {
        private readonly IAdminService _adminService;
        public ObservableCollection<TopAuthorItem> TopAuthors { get; set; }
        public ObservableCollection<TopBookItem> TopBooks { get; set; }

        public StatisticsPage()
        {
            InitializeComponent();

            var apiClient = new ApiClient(ApiEndpoints.BaseUrl);
            _adminService = new AdminService(apiClient);

            TopAuthors = [];
            TopBooks = [];

            TopAuthorsList.ItemsSource = TopAuthors;
            TopBooksList.ItemsSource = TopBooks;

            LoadStatistics();
        }

        private async void LoadStatistics()
        {
            try
            {
                ShowLoading(true);

                var stats = await _adminService.GetStatsAsync();

                Dispatcher.Invoke(() =>
                {
                    TotalUsersValue.Text = stats.Users?.Total.ToString("N0");
                    ActiveUsersValue.Text = stats.Users?.Active.ToString("N0");
                    BannedUsersValue.Text = stats.Users?.Banned.ToString("N0");
                    ModeratorsValue.Text = stats.Users?.Moderators.ToString("N0");

                    TotalBooksValue.Text = stats.Books?.Total.ToString("N0");
                    PublishedBooksValue.Text = stats.Books?.Published.ToString("N0");
                    TotalChaptersValue.Text = stats.Chapters?.Total.ToString("N0");
                    AvgChaptersValue.Text = $"{stats.Chapters?.AveragePerBook} глав на книгу";

                    TotalGenresValue.Text = stats.Genres?.Total.ToString("N0") ?? "0";
                    TotalTagsValue.Text = stats.Tags?.Total.ToString("N0") ?? "0";

                    PendingBooksValue.Text = stats.Books?.Pending.ToString("N0");
                    PendingChaptersValue.Text = stats.Chapters?.Pending.ToString("N0");
                    ModerationSuccessValue.Text = $"{stats.Summary?.ModerationSuccessRate}%";

                    TopAuthors.Clear();
                    int rank = 1;
                    foreach (var author in stats.TopAuthors!)
                    {
                        TopAuthors.Add(new TopAuthorItem
                        {
                            Rank = rank++,
                            Nickname = author.Nickname,
                            BookCount = author.BookCount
                        });
                    }

                    TopBooks.Clear();
                    rank = 1;
                    foreach (var book in stats.TopBooks!)
                    {
                        TopBooks.Add(new TopBookItem
                        {
                            Rank = rank++,
                            Title = book.Title,
                            Views = book.Views.ToString("N0")
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBoxWindow.Show($"Ошибка загрузки статистики: {ex.Message}",
                                  "Ошибка",
                                  MessageBoxIcon.Error,
                                  MessageBoxButtons.OK);
                });
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private void ShowLoading(bool isLoading)
        {
            Dispatcher.Invoke(() =>
            {
                LoadingPanel.Visibility = isLoading ? Visibility.Visible 
                                                    : Visibility.Collapsed;
            });
        }
    }
}