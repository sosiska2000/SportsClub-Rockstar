using System.Security.Cryptography;
using System.Text;
using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Database;
using Rockstar.Admin.WPF.Services.Interfaces;
using Dapper;

namespace Rockstar.Admin.WPF.Services
{
    public class AuthService : IAuthService
    {
        private readonly MySqlDbContext _dbContext;
        private User? _currentUser;
        private bool _isAuthenticated;
        private string? _token;

        public AuthService(MySqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool IsAuthenticated => _isAuthenticated;
        public User? CurrentUser => _currentUser;
        public string? Token => _token;

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            try
            {
                var sql = @"
                    SELECT id, email, password_hash, first_name, last_name, phone, age, photo, role, is_active, created_at
                    FROM users 
                    WHERE email = @Email AND is_active = TRUE AND role = 'admin'";

                var user = await _dbContext.ExecuteQueryAsync<UserDb>(sql, new { Email = email });

                if (user == null)
                {
                    return new AuthResult { Success = false, Message = "Пользователь не найден" };
                }

                // Проверка пароля (в реальном проекте используйте BCrypt)
                if (!VerifyPassword(password, user.PasswordHash))
                {
                    return new AuthResult { Success = false, Message = "Неверный пароль" };
                }

                _currentUser = new User
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    Age = user.Age,
                    Photo = user.Photo,
                    Role = user.Role == "admin" ? UserRole.Admin : UserRole.Client
                };

                _isAuthenticated = true;
                _token = GenerateJwtToken(_currentUser);

                return new AuthResult
                {
                    Success = true,
                    Token = _token,
                    User = _currentUser
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                return new AuthResult { Success = false, Message = "Ошибка подключения к базе данных" };
            }
        }

        public Task LogoutAsync()
        {
            _currentUser = null;
            _isAuthenticated = false;
            _token = null;
            return Task.CompletedTask;
        }

        // Вспомогательный класс для маппинга из БД
        private class UserDb
        {
            public int Id { get; set; }
            public string Email { get; set; } = string.Empty;
            public string PasswordHash { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string? Phone { get; set; }
            public int? Age { get; set; }
            public byte[]? Photo { get; set; }
            public string Role { get; set; } = string.Empty;
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        // Простая проверка пароля (замените на BCrypt в продакшене)
        private bool VerifyPassword(string password, string hash)
        {
            // Для тестов: если хеш начинается с $2y$ - это BCrypt
            if (hash.StartsWith("$2y$"))
            {
                // В реальном проекте используйте BCrypt.Net
                return true; // Заглушка для тестов
            }

            // Для простых тестов (не используйте в продакшене!)
            return password == "admin123";
        }

        private string GenerateJwtToken(User user)
        {
            // Упрощённая генерация токена (в реальном проекте используйте JWT)
            var tokenData = $"{user.Id}:{user.Email}:{DateTime.Now.AddDays(1).Ticks}";
            var bytes = Encoding.UTF8.GetBytes(tokenData);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}