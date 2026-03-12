using Rockstar.Admin.WPF.Models;

namespace Rockstar.Admin.WPF.Services.Interfaces
{
    public interface IServiceTypeService
    {
        Task<List<ServiceType>> GetAllAsync();
        Task<List<ServiceType>> GetByDirectionIdAsync(int directionId);
        Task<ServiceType?> GetByIdAsync(int id);
        Task<bool> CreateAsync(ServiceType serviceType);
        Task<bool> UpdateAsync(ServiceType serviceType);
        Task<bool> DeleteAsync(int id);
    }
}