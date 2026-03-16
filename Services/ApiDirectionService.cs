using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rockstar.Admin.WPF.Services;

public class ApiDirectionService : IDirectionService
{
    private readonly IApiService _apiService;

    public ApiDirectionService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<Direction>> GetAllAsync()
    {
        // TODO: реализовать
        return await Task.FromResult(new List<Direction>());
    }

    public async Task<Direction?> GetByIdAsync(int id)
    {
        // TODO: реализовать
        return await Task.FromResult<Direction?>(null);
    }

    public async Task<Direction?> GetByKeyAsync(string key)
    {
        // TODO: реализовать
        return await Task.FromResult<Direction?>(null);
    }

    public async Task<bool> CreateAsync(Direction direction)
    {
        // TODO: реализовать
        return await Task.FromResult(true);
    }

    public async Task<bool> UpdateAsync(Direction direction)
    {
        // TODO: реализовать
        return await Task.FromResult(true);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // TODO: реализовать
        return await Task.FromResult(true);
    }
}