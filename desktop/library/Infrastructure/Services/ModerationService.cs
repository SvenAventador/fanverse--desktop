using library.Core.Constants;
using library.Core.Interfaces;
using library.Core.Models.Requests;
using library.Core.Models.Responses;

namespace library.Infrastructure.Services
{
    public class ModerationService : IModerationService
    {
        private readonly IApiClient _apiClient;

        public ModerationService(IApiClient apiClient)
            => _apiClient = apiClient;

        public async Task<List<UserResponse>> GetAllUsersAsync(CancellationToken cancellationToken = default)
            => await _apiClient.GetAsync<List<UserResponse>>(ApiEndpoints.Moderation.GetAll, cancellationToken);

        public async Task<UserResponse> BanUserAsync(BanRequest request, CancellationToken cancellationToken = default)
            => await _apiClient.PostAsync<BanRequest, UserResponse>($"{ApiEndpoints.Moderation.NewBanStatus}/{request.Id}", request, cancellationToken);

        public async Task<UserResponse> UnbanUserAsync(BanRequest request, CancellationToken cancellationToken = default)
            => await _apiClient.PostAsync<BanRequest, UserResponse>($"{ApiEndpoints.Moderation.NewBanStatus}/{request.Id}", request, cancellationToken);
    }
}