using library.Core.Models.Requests;
using library.Core.Models.Responses;

namespace library.Core.Interfaces
{
    public interface IBookService
    {
        Task<List<BookResponse>> GetAllBooksAsync(CancellationToken cancellationToken = default);

        Task<BookDetailResponse> GetBookDetailByIdAsync(int bookId, CancellationToken cancellationToken = default);

        Task<List<ChapterResponse>> GetChaptersByBookIdAsync(int bookId, CancellationToken cancellationToken = default);

        Task<ChapterDetailResponse> GetChapterDetailByIdAsync(int chapterId, CancellationToken cancellationToken = default);

        Task<BookResponse> ModerateBookAsync(ModerationRequest request, CancellationToken cancellationToken = default);

        Task<ChapterResponse> ModerateChapterAsync(ModerationRequest request, CancellationToken cancellationToken = default);

        Task<BookRejectionResponse> GetBookRejectionReasonAsync(int bookId, CancellationToken cancellationToken = default);

        Task<ChapterRejectionResponse> GetChapterRejectionReasonAsync(int chapterId, CancellationToken cancellationToken = default);
    }
}