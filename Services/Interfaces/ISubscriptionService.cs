using Rockstar.Admin.WPF.Models;

namespace Rockstar.Admin.WPF.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<List<Subscription>> GetAllAsync();
        Task<List<Subscription>> GetByDirectionIdAsync(int directionId);
        Task<Subscription?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Subscription subscription);
        Task<bool> UpdateAsync(Subscription subscription);
        Task<bool> DeleteAsync(int id);
    }
}