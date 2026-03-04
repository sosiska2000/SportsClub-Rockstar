using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;

namespace Rockstar.Admin.WPF.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private User? _currentUser;
        private bool _isAuthenticated;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public bool IsAuthenticated => _isAuthenticated;
        public User? CurrentUser => _currentUser;

        // 🔑 Важно: возвращаем Task<AuthResult>
        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    "auth/login",
                    new { email, password });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResult>();
                    if (result?.Success == true)
                    {
                        _currentUser = result.User;
                        _isAuthenticated = true;
                        return result;
                    }
                }
                return new AuthResult { Success = false, Message = "Ошибка авторизации" };
            }
            catch
            {
                return new AuthResult { Success = false, Message = "Ошибка подключения" };
            }
        }

        public Task LogoutAsync()
        {
            _currentUser = null;
            _isAuthenticated = false;
            return Task.CompletedTask;
        }
    }
}