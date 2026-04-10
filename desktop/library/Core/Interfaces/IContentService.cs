using library.Core.Models.Requests;
using library.Core.Models.Responses;

namespace library.Core.Interfaces
{
    public interface IContentService
    {
        Task<List<ContentResponse>> GetAllAsync(string endpoint, CancellationToken cancellationToken = default);
        Task<ContentResponse> AddAsync(string endpoint, 
                                          ContentRequest request,
                                          CancellationToken cancellationToken = default);

        Task<ContentResponse> UpdateAsync(string endpoint, 
                                             ContentRequest request, 
                                             CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(string endpoint, int contentId, CancellationToken cancellationToken = default);
    }
}