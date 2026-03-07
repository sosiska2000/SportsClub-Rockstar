using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Dapper;

namespace Rockstar.Admin.WPF.Services.Database
{
    public class MySqlDbContext
    {
        private readonly string _connectionString;

        public MySqlDbContext(IConfiguration configuration)
        {
            // 🔑 Получаем строку подключения с fallback-значением
            _connectionString = configuration?.GetConnectionString("DefaultConnection")
                ?? "Server=127.0.0.1;Port=3306;Database=rockstar_club;Uid=root;Pwd=;Charset=utf8mb4;SslMode=none;";

            // 🔑 Проверка для отладки
            System.Diagnostics.Debug.WriteLine($"Connection String: {_connectionString}");
        }

        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public async Task<T?> ExecuteQueryAsync<T>(string sql, object? param = null)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();
            return await Dapper.SqlMapper.QueryFirstOrDefaultAsync<T>(connection, sql, param);
        }

        public async Task<IEnumerable<T>> ExecuteQueryListAsync<T>(string sql, object? param = null)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();
            return await Dapper.SqlMapper.QueryAsync<T>(connection, sql, param);
        }

        public async Task<int> ExecuteCommandAsync(string sql, object? param = null)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();
            return await Dapper.SqlMapper.ExecuteAsync(connection, sql, param);
        }

        public async Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();
            return await Dapper.SqlMapper.ExecuteScalarAsync<T>(connection, sql, param);
        }
    }
}