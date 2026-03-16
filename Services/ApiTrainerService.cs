using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;

namespace Rockstar.Admin.WPF.Services;

public class ApiTrainerService : ITrainerService
{
    private readonly IApiService _apiService;

    public ApiTrainerService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<Trainer>> GetAllAsync()
    {
        return await _apiService.GetAsync<List<Trainer>>("trainers") ?? new List<Trainer>();
    }

    public async Task<Trainer?> GetByIdAsync(int id)
    {
        return await _apiService.GetAsync<Trainer>($"trainers/{id}");
    }

    public async Task<Trainer?> GetByEmailAsync(string email)
    {
        return await _apiService.GetAsync<Trainer>($"trainers/email/{email}");
    }

    public async Task<bool> CreateAsync(Trainer trainer)
    {
        var result = await _apiService.PostAsync<Trainer>("trainers", trainer);
        return result != null;
    }

    public async Task<bool> UpdateAsync(Trainer trainer)
    {
        var result = await _apiService.PutAsync<Trainer>($"trainers/{trainer.Id}", trainer);
        return result != null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _apiService.DeleteAsync($"trainers/{id}");
    }

    public List<string> GetAvailableDirections()
    {
        return new List<string> { "yoga", "fitness", "climbing" };
    }
}