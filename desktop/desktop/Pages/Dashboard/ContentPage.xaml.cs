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
using static library.Core.Constants.ApiEndpoints;

namespace desktop.Pages.Dashboard
{
    public partial class ContentPage : Page
    {
        private readonly IContentService _contentService;

        private ObservableCollection<ContentResponse> _genres;
        private ObservableCollection<ContentResponse> _tags;
        private ObservableCollection<ContentResponse> _warnings;

        private ContentResponse _currentEditingItem;
        private string _currentEditingType;

        public ContentPage()
        {
            InitializeComponent();

            var apiClient = new ApiClient(ApiEndpoints.BaseUrl);
            _contentService = new ContentService(apiClient);

            _genres = [];
            _tags = [];
            _warnings = [];

            GenresList.ItemsSource = _genres;
            TagsList.ItemsSource = _tags;
            WarningsList.ItemsSource = _warnings;

            Loaded += async (s, e) => await LoadAllData();
        }

        private async Task LoadAllData()
        {
            await LoadGenres();
            await LoadTags();
            await LoadWarnings();
        }

        private async Task LoadGenres()
        {
            try
            {
                var genres = await _contentService.GetAllAsync(ApiEndpoints.Genre.GetAll);
                _genres.Clear();

                foreach (var genre in genres.OrderBy(g => g.Id))
                    _genres.Add(genre);
            }
            catch (Exception ex)
            {
                MessageBoxWindow.Show($"Ошибка загрузки жанров: {ex.Message}", 
                                       "Ошибка",
                                       MessageBoxIcon.Error, 
                                       MessageBoxButtons.OK);
            }
        }

        private async Task LoadTags()
        {
            try
            {
                var tags = await _contentService.GetAllAsync(ApiEndpoints.Tags.GetAll);
                _tags.Clear();

                foreach (var tag in tags.OrderBy(t => t.Id))
                    _tags.Add(tag);
            }
            catch (Exception ex)
            {
                MessageBoxWindow.Show($"Ошибка загрузки тегов: {ex.Message}", 
                                       "Ошибка",
                                       MessageBoxIcon.Error,
                                       MessageBoxButtons.OK);
            }
        }

        private async Task LoadWarnings()
        {
            try
            {
                var warnings = await _contentService.GetAllAsync(ApiEndpoints.ContentWarning.GetAll);
                _warnings.Clear();

                foreach (var warning in warnings.OrderBy(t => t.Id))
                    _warnings.Add(warning);
            }
            catch (Exception ex)
            {
                MessageBoxWindow.Show($"Ошибка загрузки предупреждений: {ex.Message}", 
                                       "Ошибка",
                                       MessageBoxIcon.Error, 
                                       MessageBoxButtons.OK);
            }
        }

        private async void AddGenreButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewGenreNameBox.Text)) 
                return;

            try
            {
                var request = new ContentRequest
                {
                    Name = NewGenreNameBox.Text.Trim(),
                };

                await _contentService.AddAsync(ApiEndpoints.Genre.Create, request);

                NewGenreNameBox.Text = "";
                await LoadGenres();

                MessageBoxWindow.Show("Жанр успешно добавлен!", 
                                      "Успех",
                                      MessageBoxIcon.Success, 
                                      MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBoxWindow.Show($"Ошибка: {ex.Message}",
                                       "Ошибка",
                                       MessageBoxIcon.Error, 
                                       MessageBoxButtons.OK);
            }
        }

        private void EditGenre_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            _currentEditingItem = button.Tag as ContentResponse;
            _currentEditingType = "genre";

            EditNameBox.Text = _currentEditingItem!.Name;
            EditDialogOverlay.Visibility = Visibility.Visible;
        }

        private async void DeleteGenre_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var id = (int)button.Tag;

            var result = MessageBoxWindow.Show("Удалить этот жанр?", 
                                               "Подтверждение",
                                               MessageBoxIcon.Question, 
                                               MessageBoxButtons.YesNo);

            if (result)
            {
                try
                {
                    await _contentService.DeleteAsync(ApiEndpoints.Genre.Delete, id);
                    await LoadGenres();

                    MessageBoxWindow.Show("Жанр удалён!", 
                                          "Успех",
                                          MessageBoxIcon.Success, 
                                          MessageBoxButtons.OK);
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

        private async void AddTagButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewTagNameBox.Text)) 
                return;

            try
            {
                var request = new ContentRequest
                {
                    Name = NewTagNameBox.Text.Trim(),
                };

                await _contentService.AddAsync(ApiEndpoints.Tags.Create, request);

                NewTagNameBox.Text = "";
                await LoadTags();

                MessageBoxWindow.Show("Тег успешно добавлен!", 
                                      "Успех",
                                      MessageBoxIcon.Success, 
                                      MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBoxWindow.Show($"Ошибка: {ex.Message}", 
                                       "Ошибка",
                                       MessageBoxIcon.Error, 
                                       MessageBoxButtons.OK);
            }
        }

        private void EditTag_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            _currentEditingItem = button?.Tag as ContentResponse;
            _currentEditingType = "tag";

            EditNameBox.Text = _currentEditingItem.Name;
            EditDialogOverlay.Visibility = Visibility.Visible;
        }

        private async void DeleteTag_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var id = (int)button.Tag;

            var result = MessageBoxWindow.Show("Удалить этот тег?", 
                                               "Подтверждение",
                                               MessageBoxIcon.Question, 
                                               MessageBoxButtons.YesNo);

            if (result)
            {
                try
                {
                    await _contentService.DeleteAsync(ApiEndpoints.Tags.Delete, id);
                    await LoadTags();

                    MessageBoxWindow.Show("Тег удалён!", 
                                          "Успех",
                                          MessageBoxIcon.Success, 
                                          MessageBoxButtons.OK);
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

        private async void AddWarningButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewWarningNameBox.Text)) 
                return;

            try
            {
                var request = new ContentRequest
                {
                    Name = NewWarningNameBox.Text.Trim(),
                };

                await _contentService.AddAsync(ApiEndpoints.ContentWarning.Create, request);

                NewWarningNameBox.Text = "";
                await LoadWarnings();

                MessageBoxWindow.Show("Предупреждение успешно добавлено!",
                                      "Успех",
                                      MessageBoxIcon.Success, 
                                      MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBoxWindow.Show($"Ошибка: {ex.Message}", 
                                       "Ошибка",
                                       MessageBoxIcon.Error,
                                       MessageBoxButtons.OK);
            }
        }

        private void EditWarning_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            _currentEditingItem = button?.Tag as ContentResponse;
            _currentEditingType = "warning";

            EditNameBox.Text = _currentEditingItem.Name;
            EditDialogOverlay.Visibility = Visibility.Visible;
        }

        private async void DeleteWarning_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var id = (int)button.Tag;

            var result = MessageBoxWindow.Show("Удалить это предупреждение?", 
                                               "Подтверждение",
                                               MessageBoxIcon.Question, 
                                               MessageBoxButtons.YesNo);

            if (result)
            {
                try
                {
                    await _contentService.DeleteAsync(ApiEndpoints.ContentWarning.Delete, id);
                    await LoadWarnings();

                    MessageBoxWindow.Show("Предупреждение удалено!", 
                                          "Успех",
                                          MessageBoxIcon.Success, 
                                          MessageBoxButtons.OK);
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

        private async void SaveEdit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EditNameBox.Text)) 
                return;

            try
            {
                var request = new ContentRequest
                {
                    Id = _currentEditingItem.Id,
                    Name = EditNameBox.Text.Trim(),
                };

                string endpoint = _currentEditingType switch
                {
                    "genre" => $"{ApiEndpoints.Genre.Update}/{request.Id}",
                    "tag" => $"{ApiEndpoints.Tags.Update}/{request.Id}",
                    "warning" => $"{ApiEndpoints.ContentWarning.Update}/{request.Id}",
                    _ => ""
                };

                await _contentService.UpdateAsync(endpoint, request);

                EditDialogOverlay.Visibility = Visibility.Collapsed;

                switch (_currentEditingType)
                {
                    case "genre":
                        await LoadGenres();
                        break;
                    case "tag":
                        await LoadTags();
                        break;
                    case "warning":
                        await LoadWarnings();
                        break;
                }

                MessageBoxWindow.Show("Изменения сохранены!", 
                                      "Успех",
                                      MessageBoxIcon.Success, 
                                      MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBoxWindow.Show($"Ошибка: {ex.Message}", 
                                       "Ошибка",
                                       MessageBoxIcon.Error, 
                                       MessageBoxButtons.OK);
            }
        }

        private void CancelEdit_Click(object sender, RoutedEventArgs e)
        {
            EditDialogOverlay.Visibility = Visibility.Collapsed;
            EditNameBox.Text = "";
            _currentEditingItem = null;
        }
    }
}