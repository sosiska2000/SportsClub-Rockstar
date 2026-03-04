using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;

namespace Rockstar.Admin.WPF.Services
{
    public class MockAuthService : IAuthService
    {
        private User? _currentUser;
        private bool _isAuthenticated;

        public bool IsAuthenticated => _isAuthenticated;
        public User? CurrentUser => _currentUser;

        // 🔑 Возвращаем Task<AuthResult>, а не Task<bool>
        public Task<AuthResult> LoginAsync(string email, string password)
        {
            return Task.Delay(300).ContinueWith(_ =>
            {
                if (email == "admin@rockstar.ru" && password == "admin123")
                {
                    _currentUser = new User
                    {
                        Id = 1,
                        Email = email,
                        FirstName = "Админ",
                        LastName = "Рокстар",
                        Role = UserRole.Admin
                    };
                    _isAuthenticated = true;
                    return new AuthResult
                    {
                        Success = true,
                        Token = "mock-token-123",
                        User = _currentUser
                    };
                }
                return new AuthResult
                {
                    Success = false,
                    Message = "Неверный email или пароль"
                };
            });
        }

        public Task LogoutAsync()
        {
            _currentUser = null;
            _isAuthenticated = false;
            return Task.CompletedTask;
        }
    }
}