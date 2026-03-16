using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;

namespace Rockstar.Admin.WPF.Services;

public class ApiAuthService : IAuthService
{
    private readonly IApiService _apiService;
    private Client? _currentUser;
    private bool _isAuthenticated;
    private string? _token;

    public ApiAuthService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public bool IsAuthenticated => _isAuthenticated;
    public Client? CurrentUser => _currentUser;
    public string? Token => _token;

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        try
        {
            var loginData = new { email, password };
            var result = await _apiService.PostAsync<AuthResponse>("auth/login", loginData);

            if (result?.Success == true && result.User != null)
            {
                _currentUser = result.User;
                _isAuthenticated = true;
                _token = result.Token;
                _apiService.SetAuthToken(result.Token!);

                return new AuthResult
                {
                    Success = true,
                    Token = result.Token,
                    User = result.User
                };
            }

            return new AuthResult
            {
                Success = false,
                Message = result?.Message ?? "Ошибка авторизации"
            };
        }
        catch (Exception ex)
        {
            return new AuthResult
            {
                Success = false,
                Message = $"Ошибка подключения: {ex.Message}"
            };
        }
    }

    public Task LogoutAsync()
    {
        _currentUser = null;
        _isAuthenticated = false;
        _token = null;
        return Task.CompletedTask;
    }

    private class AuthResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public Client? User { get; set; }
    }
}