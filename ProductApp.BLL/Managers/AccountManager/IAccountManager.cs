using Microsoft.AspNetCore.Identity;

namespace ProductApp.BLL
{
    public interface IAccountManager
    {
        Task<IdentityResult> RegisterAsync(RegisterDTO model, CancellationToken cancellationToken = default);
        Task<AuthResponseDTO?> LoginAsync(LoginDTO model, CancellationToken cancellationToken = default);
    }
}
