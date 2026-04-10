using library.Core.Interfaces;
using library.Core.Models.Requests;
using library.Core.Models.Responses;

namespace library.Infrastructure.Services
{
    public class ContentService : IContentService
    {
        private readonly IApiClient _apiClient;

        public ContentService(IApiClient apiClient)
            => _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

        public async Task<ContentResponse> AddAsync(string endpoint, ContentRequest request, CancellationToken cancellationToken = default)
                    => await _apiClient.PostAsync<ContentRequest, ContentResponse>(endpoint, request, cancellationToken);

        public Task<bool> DeleteAsync(string endpoint, int contentId, CancellationToken cancellationToken = default)
            => _apiClient.DeleteAsync($"{endpoint}/{contentId}", cancellationToken);

        public async Task<List<ContentResponse>> GetAllAsync(string endpoint, CancellationToken cancellationToken = default)
            => await _apiClient.GetAsync<List<ContentResponse>>(endpoint, cancellationToken);

        public async Task<ContentResponse> UpdateAsync(string endpoint, ContentRequest request, CancellationToken cancellationToken = default)
            => await _apiClient.PutAsync<ContentRequest, ContentResponse>(endpoint, request, cancellationToken);
    }
}