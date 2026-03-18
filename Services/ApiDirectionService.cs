using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;

namespace Rockstar.Admin.WPF.Services;

public class ApiDirectionService : IDirectionService
{
    private readonly IApiService _apiService;

    public ApiDirectionService(IApiService apiService)  // 👈 ТОЛЬКО IApiService
    {
        _apiService = apiService;
    }

    public async Task<List<Direction>> GetAllAsync()
    {
        return await _apiService.GetAsync<List<Direction>>("directions") ?? new List<Direction>();
    }

    public async Task<Direction?> GetByIdAsync(int id)
    {
        return await _apiService.GetAsync<Direction>($"directions/{id}");
    }

    public async Task<Direction?> GetByKeyAsync(string key)
    {
        return await _apiService.GetAsync<Direction>($"directions/key/{key}");
    }

    public async Task<bool> CreateAsync(Direction direction)
    {
        var result = await _apiService.PostAsync<Direction>("directions", direction);
        return result != null;
    }

    public async Task<bool> UpdateAsync(Direction direction)
    {
        var result = await _apiService.PutAsync<Direction>($"directions/{direction.Id}", direction);
        return result != null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _apiService.DeleteAsync($"directions/{id}");
    }
}