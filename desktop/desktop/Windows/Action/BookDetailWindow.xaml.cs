using library.Core.Constants;
using library.Core.Interfaces;
using library.Core.Models.Requests;
using library.Core.Models.Responses;
using library.Infrastructure.HttpClient;
using library.Infrastructure.Services;
using library.Utils;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace desktop.Windows.Action
{
    public partial class BookDetailWindow : Window
    {
        private readonly IBookService _bookService;
        private readonly int _bookId;

        public ObservableCollection<ChapterResponse> Chapters { get; set; }
        private ChapterResponse _selectedChapterForModeration;

        public BookDetailWindow(int bookId)
        {
            InitializeComponent();
            Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);

            _bookId = bookId;
            var apiClient = new ApiClient(ApiEndpoints.BaseUrl);
            _bookService = new BookService(apiClient);

            Chapters = [];
            ChaptersList.ItemsSource = Chapters;

            Loaded += async (s, e) => await LoadBookDetail();
        }

        private async Task LoadBookDetail()
        {
            try
            {
                var book = await _bookService.GetBookDetailByIdAsync(_bookId);
                var chapters = await _bookService.GetChaptersByBookIdAsync(_bookId);

                Dispatcher.Invoke(() =>
                {
                    TitleText.Text = book.Title;
                    AuthorText.Text = $"Автор: {book.User?.Nickname ?? "Неизвестен"}";
                    SummaryText.Text = book.Summary ?? "Нет описания";

                    ViewsText.Text = $"👁 {book.Views:N0} просмотров";
                    LikesText.Text = $"❤️ {book.Likes:N0} лайков";
                    CommentsText.Text = $"💬 {book.TotalComments:N0} комментариев";

                    RatingText.Text = $"⭐ Рейтинг: {GetRatingText(book.Rating)}";
                    StatusText.Text = $"📌 Статус: {(book.IsCompleted ? "Завершена" : "В процессе")}";
                    LanguageText.Text = $"🌐 Язык: {book.Language?.ToUpper()}";
                    WordCountText.Text = $"📝 Слов: {book.WordCount:N0}";

                    if (!string.IsNullOrEmpty(book.CoverUrl))
                        LoadCoverImage(book.CoverUrl);

                    Chapters.Clear();
                    foreach (var chapter in chapters.OrderBy(c => c.ChapterNumber))
                        Chapters.Add(chapter);
                });
            }
            catch (Exception ex)
            {
                MessageBoxWindow.Show($"Ошибка загрузки: {ex.Message}",
                                       "Ошибка",
                                       MessageBoxIcon.Error, 
                                       MessageBoxButtons.OK);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private async void LoadCoverImage(string coverUrl)
        {
            try
            {
                var fullUrl = (ApiEndpoints.BaseUrl + coverUrl).Replace("api", "");

                using var httpClient = new HttpClient();
                var imageBytes = await httpClient.GetByteArrayAsync(fullUrl);

                Dispatcher.Invoke(() =>
                {
                    using var stream = new MemoryStream(imageBytes);
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    CoverImage.Source = bitmap;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки обложки: {ex.Message}");
            }
        }

        private static string GetRatingText(string rating)
        {
            return rating switch
            {
                "General" => "G (Для всех)",
                "Teen" => "PG-13 (Подросткам)",
                "Mature" => "R (Взрослым)",
                "Explicit" => "NC-21 (Только для взрослых)",
                _ => rating
            };
        }

        private void Chapter_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;

            if (border?.Tag is ChapterResponse chapter)
                OpenChapter(chapter.Id);
        }

        private void ReadChapter_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.Tag is ChapterResponse chapter)
                OpenChapter(chapter.Id);
        }

        private async void OpenChapter(int chapterId)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                var chapter = await _bookService.GetChapterDetailByIdAsync(chapterId);

                var chapterWindow = new ChapterViewWindow(chapter)
                {
                    Owner = this
                };
                chapterWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBoxWindow.Show($"Ошибка загрузки главы: {ex.Message}", 
                                       "Ошибка",
                                       MessageBoxIcon.Error, 
                                       MessageBoxButtons.OK);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
            => Close();

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private async void ModerateChapter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox?.SelectedItem as ComboBoxItem;
            var chapter = comboBox?.Tag as ChapterResponse;

            if (chapter == null || selectedItem == null) return;

            var action = selectedItem.Tag as string;

            if (action == "approve")
            {
                var result = MessageBoxWindow.Show($"Одобрить главу \"{chapter.Title}\"?",
                    "Подтверждение", MessageBoxIcon.Question, MessageBoxButtons.YesNo);

                if (result)
                {
                    await ApproveChapter(chapter);
                }
            }
            else if (action == "reject")
            {
                _selectedChapterForModeration = chapter;
                RejectReasonBox.Text = "";
                ViolationTypeBox.SelectedIndex = 0;
                ChapterRejectDialog.Visibility = Visibility.Visible;
            }

            comboBox.SelectedIndex = 0;
        }

        private async Task ApproveChapter(ChapterResponse chapter)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                var request = new ModerationRequest
                {
                    Id = chapter.Id,
                    Status = "Published"
                };

                var result = await _bookService.ModerateChapterAsync(request);

                if (result != null)
                {
                    await LoadBookDetail(); 
                    MessageBoxWindow.Show($"Глава \"{chapter.Title}\" одобрена!", 
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
                Mouse.OverrideCursor = null;
            }
        }

        private async void ConfirmChapterReject_Click(object sender, RoutedEventArgs e)
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
                Mouse.OverrideCursor = Cursors.Wait;

                var request = new ModerationRequest
                {
                    Id = _selectedChapterForModeration.Id,
                    Status = "Rejected",
                    RejectionReason = RejectReasonBox.Text.Trim(),
                    ViolationType = violationType
                };

                var result = await _bookService.ModerateChapterAsync(request);

                if (result != null)
                {
                    await LoadBookDetail();
                    MessageBoxWindow.Show($"Глава \"{_selectedChapterForModeration.Title}\" отклонена!", 
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
                Mouse.OverrideCursor = null;
                ChapterRejectDialog.Visibility = Visibility.Collapsed;
            }
        }

        private void CancelChapterReject_Click(object sender, RoutedEventArgs e)
            => ChapterRejectDialog.Visibility = Visibility.Collapsed;

        private async void ShowChapterRejection_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.Tag is not ChapterResponse chapter) 
                return;

            try
            {
                var rejection = await _bookService.GetChapterRejectionReasonAsync(chapter.Id);

                if (rejection != null && 
                    rejection.HasRejection)
                {
                    var window = new ChapterRejectionReasonWindow(
                        rejection.ViolationType,
                        rejection.Reason,
                        rejection.CreatedAt
                    )
                    {
                        Owner = this
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
    }
}