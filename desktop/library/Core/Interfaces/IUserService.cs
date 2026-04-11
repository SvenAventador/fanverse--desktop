using library.Core.Models.Responses;

namespace library.Core.Interfaces
{
    public interface IUserService
    {
        Task<string> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default);

        Task<ResetPasswordResponse> ResetPasswordAsync(string email, 
                                                       string newPassword, 
                                                       CancellationToken cancellationToken = default);
    }
}