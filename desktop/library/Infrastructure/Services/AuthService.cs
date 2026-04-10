using library.Core.Constants;
using library.Core.Interfaces;
using library.Core.Models.Requests;
using library.Core.Models.Responses;
using library.Infrastructure.Storage;

namespace library.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IApiClient _apiClient;
        private string _cachedToken;

        public AuthService(IApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _cachedToken = SecureStorage.GetToken();
        }

        public bool IsAuthenticated => 
            !string.IsNullOrEmpty(AccessToken);

        public string AccessToken 
            => _cachedToken ?? SecureStorage.GetToken();

        public async Task<string> LoginAsync(string email, 
                                             string password,
                                             CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email не может быть пустым", nameof(email));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Пароль не может быть пустым", nameof(password));

            var request = new LoginRequest
            {
                Email = email.Trim(),
                Password = password
            };

            var response = await _apiClient.PostAsync<LoginRequest, LoginResponse>(
                ApiEndpoints.Auth.Login,
                request,
                cancellationToken
            );

            if (string.IsNullOrEmpty(response?.Token))
                throw new UnauthorizedAccessException("Не удалось получить токен авторизации");

            _cachedToken = response.Token;
            SecureStorage.SaveToken(response.Token);

            return response.Token;
        }

        public void Logout()
        {
            _cachedToken = null!;
            SecureStorage.ClearToken();
        }

        public async Task<bool> ValidateTokenAsync(CancellationToken cancellationToken = default)
        {
            if (!IsAuthenticated)
                return false;

            try
            {
                await _apiClient.GetAsync<object>(ApiEndpoints.Auth.Check, cancellationToken);
                return true;
            }
            catch
            {
                Logout();
                return false;
            }
        }
    }
}
