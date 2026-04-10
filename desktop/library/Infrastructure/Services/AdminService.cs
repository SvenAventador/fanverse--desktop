using library.Core.Constants;
using library.Core.Interfaces;
using library.Core.Models.Requests;
using library.Core.Models.Responses;
using library.Infrastructure.HttpClient;
using System.IO;
using System.Net.Http;
using System.Text.Json;

namespace library.Infrastructure.Services
{
    public class AdminService : IAdminService
    {
        private readonly IApiClient _apiClient;

        public AdminService(IApiClient apiClient)
            => _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

        public async Task<AdminResponse> AddAdminAsync(AdminRequest request, 
                                                       CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAdminAsync(int adminId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AdminResponse>> GetAllAdminsAsync(CancellationToken cancellationToken = default)
            => await _apiClient.GetAsync<List<AdminResponse>>(ApiEndpoints.Admin.GetAll, cancellationToken);

        public async Task<AdminResponse> GetCurrentAdminAsync(CancellationToken cancellationToken = default)
            => await _apiClient.GetAsync<AdminResponse>(ApiEndpoints.Admin.GetMe, cancellationToken);

        public async Task<AvatarUploadResponse> UploadAvatarAsync(string filePath, 
                                                                  string fileParamName,
                                                                  CancellationToken cancellationToken = default)
        {
            using var formData = new MultipartFormDataContent();

            var fileBytes = File.ReadAllBytes(filePath);
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

            formData.Add(fileContent, fileParamName, Path.GetFileName(filePath));

            var response = await _apiClient.PostMultipartAsync(ApiEndpoints.Admin.SetAvatar, 
                                                               formData, 
                                                               cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Ошибка: {response.StatusCode}");

            return JsonSerializer.Deserialize<AvatarUploadResponse>(content)!;
        }
    }
}