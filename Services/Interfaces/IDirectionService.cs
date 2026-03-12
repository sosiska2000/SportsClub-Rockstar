using Rockstar.Admin.WPF.Models;

namespace Rockstar.Admin.WPF.Services.Interfaces
{
    public interface IDirectionService
    {
        Task<List<Direction>> GetAllAsync();
        Task<Direction?> GetByIdAsync(int id);
        Task<Direction?> GetByKeyAsync(string key);
        Task<bool> CreateAsync(Direction direction);
        Task<bool> UpdateAsync(Direction direction);
        Task<bool> DeleteAsync(int id);
    }
}