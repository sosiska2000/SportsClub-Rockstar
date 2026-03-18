using System.Threading.Tasks;

namespace Rockstar.Admin.WPF.Services.Interfaces
{
    public interface IApiService
    {
        Task<T?> GetAsync<T>(string endpoint);
        Task<T?> PostAsync<T>(string endpoint, object data);
        Task<T?> PutAsync<T>(string endpoint, object data);
        Task<bool> DeleteAsync(string endpoint);
        void SetAuthToken(string token);
    }
}