using library.Core.Constants;
using library.Core.Interfaces;
using library.Core.Models.Responses;

namespace library.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IApiClient _apiClient;

        public UserService(IApiClient apiClient)
            => _apiClient = apiClient;

        public async Task<string> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
            => await _apiClient.PostAsync<object, string>($"{ApiEndpoints.Password.ForgotPassword}", new { email }, cancellationToken);


        public async Task<ResetPasswordResponse> ResetPasswordAsync(string email, 
                                                                    string newPassword, 
                                                                    CancellationToken cancellationToken = default)
            => await _apiClient.PostAsync<object, ResetPasswordResponse>($"{ApiEndpoints.Password.ResetPassword}", new { email, password = newPassword }, cancellationToken);
    }
}
