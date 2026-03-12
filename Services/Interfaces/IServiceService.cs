using Rockstar.Admin.WPF.Models;

namespace Rockstar.Admin.WPF.Services.Interfaces
{
    public interface IServiceService
    {
        Task<List<Service>> GetAllAsync();
        Task<List<Service>> GetByDirectionIdAsync(int directionId);
        Task<Service?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Service service);
        Task<bool> UpdateAsync(Service service);
        Task<bool> DeleteAsync(int id);
    }
}