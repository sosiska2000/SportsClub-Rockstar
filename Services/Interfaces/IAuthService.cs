using Rockstar.Admin.WPF.Models;

namespace Rockstar.Admin.WPF.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(string email, string password);
        Task LogoutAsync();
        bool IsAuthenticated { get; }
        Client? CurrentUser { get; }  // 🔑 Было User?, стало Client?
    }
}