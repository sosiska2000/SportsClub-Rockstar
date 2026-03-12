using Dapper;

namespace Rockstar.Admin.WPF.Services.Database
{
    public class DatabaseTestService
    {
        private readonly MySqlDbContext _dbContext;

        public DatabaseTestService(MySqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var sql = "SELECT 1";
                var result = await _dbContext.ExecuteScalarAsync<int>(sql);
                return result == 1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DB Connection Error: {ex.Message}");
                return false;
            }
        }

        public async Task<int> GetUsersCountAsync()
        {
            var sql = "SELECT COUNT(*) FROM users";
            return await _dbContext.ExecuteScalarAsync<int>(sql);
        }

        public async Task<int> GetTrainersCountAsync()
        {
            var sql = "SELECT COUNT(*) FROM trainers WHERE is_active = TRUE";
            return await _dbContext.ExecuteScalarAsync<int>(sql);
        }
    }
}