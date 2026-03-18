using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.Json;
using System.Linq;

namespace Rockstar.Admin.WPF.Services
{
    public class ApiServiceService : IServiceService
    {
        private readonly IApiService _apiService;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiServiceService(IApiService apiService)
        {
            _apiService = apiService;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<Service>> GetAllAsync()
        {
            try
            {
                Debug.WriteLine("📥 Fetching all services");
                return await _apiService.GetAsync<List<Service>>("services") ?? new List<Service>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 Error in GetAllAsync: {ex.Message}");
                return new List<Service>();
            }
        }

        public async Task<List<Service>> GetByDirectionIdAsync(int directionId)
        {
            try
            {
                Debug.WriteLine($"📥 Fetching services for direction {directionId}");
                return await _apiService.GetAsync<List<Service>>($"services/by-direction/{directionId}")
                       ?? new List<Service>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 Error in GetByDirectionIdAsync: {ex.Message}");
                return new List<Service>();
            }
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            try
            {
                Debug.WriteLine($"📥 Fetching service {id}");
                return await _apiService.GetAsync<Service>($"services/{id}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 Error in GetByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateAsync(Service service)
        {
            try
            {
                Debug.WriteLine($"📤 Creating service: {service.Name}");

                // Убедимся, что CreatedAt не отправляется (API сам установит)
                service.Id = 0;

                var result = await _apiService.PostAsync<Service>("services", service);
                return result != null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 Error in CreateAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Service service)
        {
            try
            {
                Debug.WriteLine($"📤 Updating service: {service.Id}");
                var result = await _apiService.PutAsync<Service>($"services/{service.Id}", service);
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
                Debug.WriteLine($"📤 Deleting service: {id}");
                return await _apiService.DeleteAsync($"services/{id}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 Error in DeleteAsync: {ex.Message}");
                return false;
            }
        }
    }
}