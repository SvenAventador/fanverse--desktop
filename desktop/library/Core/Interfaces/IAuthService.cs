namespace library.Core.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
        void Logout();
        bool IsAuthenticated { get; }
        string AccessToken { get; }
        Task<bool> ValidateTokenAsync(CancellationToken cancellationToken = default);
    }
}