using library.Core.Models.Requests;
using library.Core.Models.Responses;

namespace library.Core.Interfaces
{
    public interface IAdminService
    {
        Task<AdminResponse> GetCurrentAdminAsync(CancellationToken cancellationToken = default);

        Task<AvatarUploadResponse> UploadAvatarAsync(string filePath, 
                                                     string fileParamName,
                                                     CancellationToken cancellationToken = default);

        Task<List<AdminResponse>> GetAllAdminsAsync(CancellationToken cancellationToken = default);

        Task<AdminResponse> AddAdminAsync(AdminRequest request, CancellationToken cancellationToken = default);

        Task<AdminResponse> UpdateAdminAsync(AdminRequest request, CancellationToken cancellationToken = default);

        Task<bool> DeleteAdminAsync(int adminId, CancellationToken cancellationToken = default);

        Task<StatisticsResponse> GetStatsAsync(CancellationToken cancellationToken = default);
    }
}