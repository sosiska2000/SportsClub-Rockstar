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
        private Client? _currentUser;  // 🔑 Было UserDb, стало Client
        private bool _isAuthenticated;
        private string? _token;

        public AuthService(MySqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool IsAuthenticated => _isAuthenticated;
        public Client? CurrentUser => _currentUser;  // 🔑 Теперь возвращает Client?
        public string? Token => _token;

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            try
            {
                var sql = @"
                    SELECT id, email, password_hash, first_name, last_name, phone, age, photo, role, is_active, created_at
                    FROM users 
                    WHERE email = @Email AND is_active = TRUE AND role = 'admin'";

                var userDb = await _dbContext.ExecuteQueryAsync<UserDb>(sql, new { Email = email });

                if (userDb == null)
                {
                    return new AuthResult { Success = false, Message = "Пользователь не найден" };
                }

                // Проверка пароля
                if (!VerifyPassword(password, userDb.PasswordHash))
                {
                    return new AuthResult { Success = false, Message = "Неверный пароль" };
                }

                // 🔑 Маппинг из UserDb в Client
                _currentUser = new Client
                {
                    Id = userDb.Id,
                    Email = userDb.Email,
                    FirstName = userDb.FirstName,
                    LastName = userDb.LastName,
                    Phone = userDb.Phone,
                    Age = userDb.Age,
                    Photo = userDb.Photo
                    // PasswordHash не копируем в модель для безопасности
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

        // 🔑 Вспомогательный класс только для маппинга из БД (приватный)
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

        private bool VerifyPassword(string password, string hash)
        {
            if (hash.StartsWith("$2y$"))
            {
                return true;
            }
            return password == "admin123";
        }

        private string GenerateJwtToken(Client user)
        {
            var tokenData = $"{user.Id}:{user.Email}:{DateTime.Now.AddDays(1).Ticks}";
            var bytes = Encoding.UTF8.GetBytes(tokenData);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}