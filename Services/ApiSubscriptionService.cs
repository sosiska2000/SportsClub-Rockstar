using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.Json;
using System.Linq;

namespace Rockstar.Admin.WPF.Services
{
    public class ApiSubscriptionService : ISubscriptionService
    {
        private readonly IApiService _apiService;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiSubscriptionService(IApiService apiService)
        {
            _apiService = apiService;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<Subscription>> GetAllAsync()
        {
            try
            {
                Debug.WriteLine("📥 Fetching all subscriptions");
                return await _apiService.GetAsync<List<Subscription>>("subscriptions") ?? new List<Subscription>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 Error in GetAllAsync: {ex.Message}");
                return new List<Subscription>();
            }
        }

        public async Task<List<Subscription>> GetByDirectionIdAsync(int directionId)
        {
            try
            {
                Debug.WriteLine($"📥 Fetching subscriptions for direction {directionId}");
                return await _apiService.GetAsync<List<Subscription>>($"subscriptions/by-direction/{directionId}")
                       ?? new List<Subscription>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 Error in GetByDirectionIdAsync: {ex.Message}");
                return new List<Subscription>();
            }
        }

        public async Task<Subscription?> GetByIdAsync(int id)
        {
            try
            {
                Debug.WriteLine($"📥 Fetching subscription {id}");
                return await _apiService.GetAsync<Subscription>($"subscriptions/{id}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 Error in GetByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateAsync(Subscription subscription)
        {
            try
            {
                Debug.WriteLine($"📤 Creating subscription: {subscription.Name}");

                // Убедимся, что CreatedAt не отправляется (API сам установит)
                subscription.Id = 0;

                var result = await _apiService.PostAsync<Subscription>("subscriptions", subscription);
                return result != null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 Error in CreateAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Subscription subscription)
        {
            try
            {
                Debug.WriteLine($"📤 Updating subscription: {subscription.Id}");
                var result = await _apiService.PutAsync<Subscription>($"subscriptions/{subscription.Id}", subscription);
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
                Debug.WriteLine($"📤 Deleting subscription: {id}");
                return await _apiService.DeleteAsync($"subscriptions/{id}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 Error in DeleteAsync: {ex.Message}");
                return false;
            }
        }
    }
}