using desktop.Windows.Action;
using library.Core.Constants;
using library.Core.Interfaces;
using library.Core.Models.Requests;
using library.Core.Models.Responses;
using library.Infrastructure.HttpClient;
using library.Infrastructure.Services;
using library.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace desktop.Pages.Dashboard
{
    public partial class BooksPage : Page
    {
        private readonly IBookService _bookService;
        private ObservableCollection<BookResponse> _books;
        private BookResponse _selectedBookForModeration;

        public BooksPage()
        {
            InitializeComponent();

            var apiClient = new ApiClient(ApiEndpoints.BaseUrl);
            _bookService = new BookService(apiClient);

            _books = [];
            BooksDataGrid.ItemsSource = _books;

            Loaded += async (s, e) => await LoadBooks();
        }

        private async Task LoadBooks()
        {
            try
            {
                ShowLoading(true);

                var books = await _bookService.GetAllBooksAsync();

                _books.Clear();

                foreach (var book in books.OrderBy(b => b.Id))
                    _books.Add(book);

                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки книг: {ex.Message}", 
                                 "Ошибка",
                                 MessageBoxButton.OK, 
                                 MessageBoxImage.Error);
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private void ApplyFilters()
        {
            if (BooksDataGrid == null) 
                return;

            var searchText = SearchBox?.Text?.Trim().ToLower() ?? "";
            var filter = (StatusFilterBox?.SelectedItem as ComboBoxItem)?.Content?.ToString();

            var filtered = _books.AsEnumerable();

            if (!string.IsNullOrEmpty(searchText))
            {
                filtered = filtered.Where(b =>
                    b.Title?.ToLower(System.Globalization.CultureInfo.CurrentCulture).Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
                    (b.User?.Nickname?.ToLower().Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true));
            }

            if (filter != null && filter != "Все статусы")
            {
                var statusMap = new Dictionary<string, string>
                {
                    ["На рассмотрении"] = "Submitted",
                    ["Опубликовано"] = "Published",
                    ["Отклонено"] = "Rejected",
                    ["Черновик"] = "Draft"
                };

                if (statusMap.TryGetValue(filter, out var status))
                    filtered = filtered.Where(b => b.ModerationStatus == status);
            }

            BooksDataGrid.ItemsSource = filtered.ToList();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyFilters();

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => ApplyFilters();

        private async void ModerateBook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            if (comboBox?.Tag is not BookResponse book ||
                comboBox?.SelectedItem is not ComboBoxItem selectedItem)
                return;

            var action = selectedItem.Tag as string;

            if (action == "approve")
            {
                var result = MessageBoxWindow.Show($"Одобрить книгу \"{book.Title}\"?",
                                                    "Подтверждение", 
                                                    MessageBoxIcon.Question, 
                                                    MessageBoxButtons.YesNo);

                if (result)
                    await ApproveBook(book);
            }
            else if (action == "reject")
            {
                _selectedBookForModeration = book;
                RejectReasonBox.Text = "";
                RejectDialog.Visibility = Visibility.Visible;
            }

            comboBox.SelectedIndex = 0;
        }

        private async Task ApproveBook(BookResponse book)
        {
            try
            {
                ShowLoading(true);

                var request = new ModerationRequest
                {
                    Id = book.Id,
                    Status = "Published"
                };

                var result = await _bookService.ModerateBookAsync(request);

                if (result != null)
                {
                    await LoadBooks();
                    MessageBoxWindow.Show($"Книга \"{book.Title}\" одобрена!", 
                                           "Успех",
                                           MessageBoxIcon.Success, 
                                           MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBoxWindow.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxIcon.Error, MessageBoxButtons.OK);
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private async void ConfirmReject_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RejectReasonBox.Text))
            {
                MessageBoxWindow.Show("Укажите причину отклонения!", 
                                      "Внимание",
                                      MessageBoxIcon.Info, 
                                      MessageBoxButtons.OK);
                return;
            }

            var violationItem = ViolationTypeBox.SelectedItem as ComboBoxItem;
            var violationType = violationItem?.Tag as string ?? "Other";

            try
            {
                ShowLoading(true);

                var request = new ModerationRequest
                {
                    Id = _selectedBookForModeration.Id,
                    Status = "Rejected",
                    RejectionReason = RejectReasonBox.Text.Trim(),
                    ViolationType = violationType
                };

                var result = await _bookService.ModerateBookAsync(request);

                if (result != null)
                {
                    await LoadBooks();
                    MessageBoxWindow.Show($"Книга \"{_selectedBookForModeration.Title}\" отклонена!", 
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
                RejectDialog.Visibility = Visibility.Collapsed;
            }
        }

        private void CancelReject_Click(object sender, RoutedEventArgs e)
            => RejectDialog.Visibility = Visibility.Collapsed;

        private void ViewBook_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.Tag is BookResponse book)
            {
                var detailWindow = new BookDetailWindow(book.Id)
                {
                    Owner = Window.GetWindow(this)
                };
                detailWindow.ShowDialog();
            }
        }

        private async void ShowRejection_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.Tag is not BookResponse book) 
                return;

            try
            {
                var rejection = await _bookService.GetBookRejectionReasonAsync(book.Id);

                if (rejection != null && 
                    rejection.HasRejection)
                {
                    var window = new RejectionReasonWindow(
                        rejection.ViolationType,
                        rejection.Reason,
                        rejection.CreatedAt
                    )
                    {
                        Owner = Window.GetWindow(this)
                    };
                    window.ShowDialog();
                }
                else
                {
                    MessageBoxWindow.Show("Причина отклонения не найдена", 
                                          "Информация",
                                          MessageBoxIcon.Info, 
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
        }

        private void ShowLoading(bool isLoading)
        {
            Dispatcher.Invoke(() =>
            {
                LoadingPanel.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
                BooksDataGrid.Visibility = isLoading ? Visibility.Collapsed : Visibility.Visible;
            });
        }
    }
}