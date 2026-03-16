using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rockstar.Admin.WPF.Services;

public class ApiServiceService : IServiceService
{
    private readonly IApiService _apiService;

    public ApiServiceService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<Service>> GetAllAsync()
    {
        // TODO: реализовать
        return await Task.FromResult(new List<Service>());
    }

    public async Task<List<Service>> GetByDirectionIdAsync(int directionId)
    {
        // TODO: реализовать
        return await Task.FromResult(new List<Service>());
    }

    public async Task<Service?> GetByIdAsync(int id)
    {
        // TODO: реализовать
        return await Task.FromResult<Service?>(null);
    }

    public async Task<bool> CreateAsync(Service service)
    {
        // TODO: реализовать
        return await Task.FromResult(true);
    }

    public async Task<bool> UpdateAsync(Service service)
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