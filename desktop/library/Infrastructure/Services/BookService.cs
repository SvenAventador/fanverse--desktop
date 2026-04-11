using library.Core.Constants;
using library.Core.Interfaces;
using library.Core.Models.Requests;
using library.Core.Models.Responses;

namespace library.Infrastructure.Services
{
    public class BookService : IBookService
    {
        private readonly IApiClient _apiClient;

        public BookService(IApiClient apiClient)
            => _apiClient = apiClient;

        public async Task<List<BookResponse>> GetAllBooksAsync(CancellationToken cancellationToken = default)
            => await _apiClient.GetAsync<List<BookResponse>>(ApiEndpoints.Books.GetAll, cancellationToken);

        public async Task<BookDetailResponse> GetBookDetailByIdAsync(int bookId, CancellationToken cancellationToken = default)
            => await _apiClient.GetAsync<BookDetailResponse>($"{ApiEndpoints.Books.GetById}/{bookId}", cancellationToken);

        public async Task<List<ChapterResponse>> GetChaptersByBookIdAsync(int bookId, CancellationToken cancellationToken = default)
            => await _apiClient.GetAsync<List<ChapterResponse>>(ApiEndpoints.Books.GetChapters(bookId), cancellationToken);

        public async Task<ChapterDetailResponse> GetChapterDetailByIdAsync(int chapterId, CancellationToken cancellationToken = default)
            => await _apiClient.GetAsync<ChapterDetailResponse>($"{ApiEndpoints.Chapters.GetById}/{chapterId}", cancellationToken);

        public async Task<BookResponse> ModerateBookAsync(ModerationRequest request, CancellationToken cancellationToken = default)
            => await _apiClient.PostAsync<ModerationRequest, BookResponse>(ApiEndpoints.Books.Moderate, request, cancellationToken);

        public async Task<ChapterResponse> ModerateChapterAsync(ModerationRequest request, CancellationToken cancellationToken = default)
            => await _apiClient.PostAsync<ModerationRequest, ChapterResponse>(ApiEndpoints.Chapters.Moderate, request, cancellationToken);

        public async Task<BookRejectionResponse> GetBookRejectionReasonAsync(int bookId, CancellationToken cancellationToken = default)
            => await _apiClient.GetAsync<BookRejectionResponse>($"{ApiEndpoints.Books.GetById}/{bookId}/rejection", cancellationToken);

        public async Task<ChapterRejectionResponse> GetChapterRejectionReasonAsync(int chapterId, CancellationToken cancellationToken = default)
            => await _apiClient.GetAsync<ChapterRejectionResponse>($"{ApiEndpoints.Chapters.GetById}/{chapterId}/rejection", cancellationToken);
    }
}