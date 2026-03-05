using System.Collections.Generic;
using System.Threading.Tasks;
using Rockstar.Admin.WPF.Models;

namespace Rockstar.Admin.WPF.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<List<Trainer>> GetAllAsync();
        Task<Trainer?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Trainer trainer);
        Task<bool> UpdateAsync(Trainer trainer);
        Task<bool> DeleteAsync(int id);
        List<string> GetAvailableDirections();
    }
}