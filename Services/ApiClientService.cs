using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;
using System.Diagnostics;
using System.Linq;

namespace Rockstar.Admin.WPF.Services
{
    public class ApiClientService : IClientService
    {
        private readonly IApiService _apiService;  // Используем IApiService вместо HttpClient
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiClientService(IApiService apiService)
        {
            _apiService = apiService;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<Client>> GetAllAsync()
        {
            try
            {
                Debug.WriteLine("📥 Fetching all clients from API");

                // Используем новый эндпоинт для списка клиентов
                var users = await _apiService.GetAsync<List<UserListDto>>("users");

                return users?.Select(u => MapToClient(u)).ToList() ?? new List<Client>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 Error in GetAllAsync: {ex.Message}");
                return new List<Client>();
            }
        }

        public async Task<bool> CreateAsync(Client client)
        {
            try
            {
                Debug.WriteLine($"📤 Creating client: {client.Email}");

                // Для создания используем RegisterRequest
                var registerRequest = new
                {
                    Email = client.Email,
                    Password = client.PasswordHash, // Временно, пока не будет нормального пароля
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Phone = client.Phone,
                    Age = client.Age,
                    PhotoBase64 = client.Photo != null ? Convert.ToBase64String(client.Photo) : null
                };

                var result = await _apiService.PostAsync<AuthResponse>("auth/register", registerRequest);
                return result?.Success == true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 Error in CreateAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Client client)
        {
            try
            {
                Debug.WriteLine($"📤 Updating client: {client.Id}");

                var updateDto = new
                {
                    client.FirstName,
                    client.LastName,
                    client.Phone,
                    client.Age,
                    PhotoBase64 = client.Photo != null ? Convert.ToBase64String(client.Photo) : null
                };

                var result = await _apiService.PutAsync<object>($"users/{client.Id}", updateDto);
                return result != null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 Error in UpdateAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                Debug.WriteLine($"📤 Deleting client: {id}");

                // В API используется мягкое удаление (IsActive = false)
                return await _apiService.DeleteAsync($"users/{id}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 Error in DeleteAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Client>> GetAllIncludingDeletedAsync()
        {
            try
            {
                // Можно добавить параметр для получения всех
                var users = await _apiService.GetAsync<List<UserListDto>>("users?includeInactive=true");
                return users?.Select(u => MapToClient(u)).ToList() ?? new List<Client>();
            }
            catch
            {
                return await GetAllAsync(); // Fallback
            }
        }

        private Client MapToClient(UserListDto dto)
        {
            return new Client
            {
                Id = dto.Id,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                Age = dto.Age,
                Photo = !string.IsNullOrEmpty(dto.PhotoBase64)
                    ? Convert.FromBase64String(dto.PhotoBase64)
                    : null,
                IsActive = dto.IsActive,
                Role = dto.Role
            };
        }

        private class UserListDto
        {
            public int Id { get; set; }
            public string Email { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string? Phone { get; set; }
            public int? Age { get; set; }
            public string? PhotoBase64 { get; set; }
            public string Role { get; set; } = string.Empty;
            public bool IsActive { get; set; }
        }

        private class AuthResponse
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
        }
    }
}