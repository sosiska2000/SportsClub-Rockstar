using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;

namespace Rockstar.Admin.WPF.Services;

public class ApiClientService : IClientService
{
    private readonly IApiService _apiService;

    public ApiClientService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<Client>> GetAllAsync()
    {
        return await _apiService.GetAsync<List<Client>>("users") ?? new List<Client>();
    }

    public async Task<bool> CreateAsync(Client client)
    {
        var result = await _apiService.PostAsync<Client>("users", client);
        return result != null;
    }

    public async Task<bool> UpdateAsync(Client client)
    {
        var result = await _apiService.PutAsync<Client>($"users/{client.Id}", client);
        return result != null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _apiService.DeleteAsync($"users/{id}");
    }

    public Task<List<Client>> GetAllIncludingDeletedAsync()
    {
        // В API пока нет такого эндпоинта
        return GetAllAsync();
    }
}