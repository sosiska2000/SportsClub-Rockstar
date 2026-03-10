using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;

namespace Rockstar.Admin.WPF.Services
{
    public class MockAuthService : IAuthService
    {
        // 🔑 Исправлено: _currentClient → _currentUser
        private Client? _currentUser;
        private bool _isAuthenticated;

        public bool IsAuthenticated => _isAuthenticated;
        public Client? CurrentUser => _currentUser;

        public Task<AuthResult> LoginAsync(string email, string password)
        {
            return Task.Delay(300).ContinueWith(_ =>
            {
                if (email == "admin@rockstar.ru" && password == "admin123")
                {
                    _currentUser = new Client
                    {
                        Id = 1,
                        Email = email,
                        FirstName = "Админ",
                        LastName = "Рокстар",
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
            _currentUser = null;  // 🔑 Исправлено
            _isAuthenticated = false;
            return Task.CompletedTask;
        }
    }
}