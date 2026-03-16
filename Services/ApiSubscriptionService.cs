using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rockstar.Admin.WPF.Services;

public class ApiSubscriptionService : ISubscriptionService
{
    private readonly IApiService _apiService;

    public ApiSubscriptionService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<Subscription>> GetAllAsync()
    {
        // TODO: реализовать
        return await Task.FromResult(new List<Subscription>());
    }

    public async Task<List<Subscription>> GetByDirectionIdAsync(int directionId)
    {
        // TODO: реализовать
        return await Task.FromResult(new List<Subscription>());
    }

    public async Task<Subscription?> GetByIdAsync(int id)
    {
        // TODO: реализовать
        return await Task.FromResult<Subscription?>(null);
    }

    public async Task<bool> CreateAsync(Subscription subscription)
    {
        // TODO: реализовать
        return await Task.FromResult(true);
    }

    public async Task<bool> UpdateAsync(Subscription subscription)
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