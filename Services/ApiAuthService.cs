using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;
using System.Diagnostics;

namespace Rockstar.Admin.WPF.Services
{
    public class ApiAuthService : IAuthService
    {
        private readonly IApiService _apiService;
        private Client? _currentUser;
        private bool _isAuthenticated;
        private string? _token;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiAuthService(IApiService apiService)
        {
            _apiService = apiService;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public bool IsAuthenticated => _isAuthenticated;
        public Client? CurrentUser => _currentUser;
        public string? Token => _token;

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            try
            {
                Debug.WriteLine($"🔐 [API] Login attempt: {email}");

                var loginRequest = new { email, password };

                // 👇 ВАЖНО: Используем PostAsync для отправки запроса
                var authResponse = await _apiService.PostAsync<AuthResponse>("auth/login", loginRequest);

                if (authResponse == null)
                {
                    Debug.WriteLine("❌ No response from server");
                    return new AuthResult
                    {
                        Success = false,
                        Message = "Не удалось подключиться к серверу"
                    };
                }

                if (!authResponse.Success)
                {
                    Debug.WriteLine($"❌ Login failed: {authResponse.Message}");
                    return new AuthResult
                    {
                        Success = false,
                        Message = authResponse.Message ?? "Ошибка авторизации"
                    };
                }

                Debug.WriteLine($"✅ Login successful, token received: {authResponse.Token?.Substring(0, Math.Min(20, authResponse.Token?.Length ?? 0))}...");

                // Сохраняем токен
                _token = authResponse.Token;
                _isAuthenticated = true;

                // Устанавливаем токен в ApiService для всех последующих запросов
                if (!string.IsNullOrEmpty(_token))
                {
                    _apiService.SetAuthToken(_token);
                }

                // Маппим пользователя
                if (authResponse.User != null)
                {
                    _currentUser = new Client
                    {
                        Id = authResponse.User.Id,
                        Email = authResponse.User.Email,
                        FirstName = authResponse.User.FirstName,
                        LastName = authResponse.User.LastName,
                        Phone = authResponse.User.Phone,
                        Age = authResponse.User.Age,
                        Role = authResponse.User.Role
                    };

                    // Если есть фото в Base64, конвертируем в byte[]
                    if (!string.IsNullOrEmpty(authResponse.User.PhotoBase64))
                    {
                        try
                        {
                            _currentUser.Photo = Convert.FromBase64String(authResponse.User.PhotoBase64);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Photo decode error: {ex.Message}");
                        }
                    }
                }

                return new AuthResult
                {
                    Success = true,
                    Token = _token,
                    User = _currentUser
                };
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"💥 [API] Network error: {ex.Message}");
                return new AuthResult
                {
                    Success = false,
                    Message = "Не удалось подключиться к серверу. Проверьте, запущен ли API."
                };
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine($"💥 [API] Timeout error: {ex.Message}");
                return new AuthResult
                {
                    Success = false,
                    Message = "Превышено время ожидания ответа от сервера."
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 [API] Unexpected error: {ex.Message}");
                return new AuthResult
                {
                    Success = false,
                    Message = "Произошла неизвестная ошибка"
                };
            }
        }

        public Task LogoutAsync()
        {
            _currentUser = null;
            _isAuthenticated = false;
            _token = null;

            // Очищаем токен в ApiService
            _apiService.SetAuthToken(null);

            return Task.CompletedTask;
        }

        // Внутренний класс для ответа от API
        private class AuthResponse
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
            public string? Token { get; set; }
            public UserDto? User { get; set; }
        }

        private class UserDto
        {
            public int Id { get; set; }
            public string Email { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string? Phone { get; set; }
            public int? Age { get; set; }
            public string? PhotoBase64 { get; set; }
            public string Role { get; set; } = string.Empty;
        }
    }
}