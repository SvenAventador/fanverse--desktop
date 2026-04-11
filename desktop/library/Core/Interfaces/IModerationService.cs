using library.Core.Models.Requests;
using library.Core.Models.Responses;

namespace library.Core.Interfaces
{
    public interface IModerationService
    {
        Task<List<UserResponse>> GetAllUsersAsync(CancellationToken cancellationToken = default);
        Task<UserResponse> BanUserAsync(BanRequest request, 
                                        CancellationToken cancellationToken = default);
        Task<UserResponse> UnbanUserAsync(BanRequest request,
                                          CancellationToken cancellationToken = default);
    }
}