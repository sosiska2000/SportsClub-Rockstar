using Dapper;
using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Database;
using Rockstar.Admin.WPF.Services.Interfaces;
using System.Diagnostics;

namespace Rockstar.Admin.WPF.Services
{
    public class ClientService : IClientService
    {
        private readonly MySqlDbContext _dbContext;

        public ClientService(MySqlDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<List<Client>> GetAllAsync()
        {
            try
            {
                const string sql = @"
            SELECT 
                id AS Id,
                first_name AS FirstName,
                last_name AS LastName,
                email AS Email,
                password_hash AS PasswordHash,
                phone AS Phone,
                age AS Age,
                photo AS Photo,
                created_at AS CreatedAt,
                is_active AS IsActive
            FROM users
            WHERE role = 'client' AND is_active = 1
            ORDER BY id DESC";

                var clients = await _dbContext.ExecuteQueryListAsync<Client>(sql);

                Debug.WriteLine($"GetAllAsync: loaded {clients.Count()} clients");

                foreach (var c in clients)
                {
                    Debug.WriteLine($"Client {c.Id}: '{c.FirstName}' '{c.LastName}' <{c.Email}> FullName: '{c.FullName}'");
                }

                return clients.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetAllAsync Error: {ex.Message}");
                return new List<Client>();
            }
        }

        public async Task<bool> CreateAsync(Client client)
        {
            try
            {
                if (client == null)
                    throw new ArgumentNullException(nameof(client));

                Debug.WriteLine($"=== CREATE CLIENT DEBUG ===");
                Debug.WriteLine($"FirstName: '{client.FirstName}'");
                Debug.WriteLine($"LastName: '{client.LastName}'");
                Debug.WriteLine($"Email: '{client.Email}'");
                Debug.WriteLine($"Phone: '{client.Phone}'");
                Debug.WriteLine($"Age: '{client.Age}'");
                Debug.WriteLine($"Photo length: {client.Photo?.Length ?? 0}");
                Debug.WriteLine($"==========================");

                if (await EmailExistsAsync(client.Email))
                {
                    Debug.WriteLine($"ERROR: Email '{client.Email}' already exists!");
                    return false;
                }

                const string sql = @"
            INSERT INTO users 
            (first_name, last_name, email, password_hash, phone, age, photo, role, is_active, created_at)
            VALUES 
            (@FirstName, @LastName, @Email, @PasswordHash, @Phone, @Age, @Photo, 'client', 1, NOW())";

                var parameters = new
                {
                    FirstName = client.FirstName ?? "",
                    LastName = client.LastName ?? "",
                    Email = client.Email ?? "",
                    PasswordHash = client.PasswordHash ?? "",
                    Phone = client.Phone,
                    Age = client.Age,
                    Photo = client.Photo
                };

                var result = await _dbContext.ExecuteCommandAsync(sql, parameters);

                Debug.WriteLine($"CreateAsync: {result} rows affected");
                return result > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CreateAsync Error: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateAsync(Client client)
        {
            try
            {
                if (client == null)
                    throw new ArgumentNullException(nameof(client));

                if (await EmailExistsForOtherClientAsync(client.Email, client.Id))
                {
                    Debug.WriteLine($"ERROR: Email '{client.Email}' already exists for another client!");
                    return false;
                }

                const string sql = @"
            UPDATE users 
            SET first_name = @FirstName, 
                last_name = @LastName, 
                email = @Email,
                password_hash = @PasswordHash,
                phone = @Phone, 
                age = @Age, 
                photo = @Photo
            WHERE id = @Id";

                var parameters = new
                {
                    client.Id,
                    client.FirstName,
                    client.LastName,
                    client.Email,
                    client.PasswordHash,
                    client.Phone,
                    client.Age,
                    client.Photo
                };

                var result = await _dbContext.ExecuteCommandAsync(sql, parameters);

                Debug.WriteLine($"UpdateAsync: {result} rows affected for ID {client.Id}");
                return result > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UpdateAsync Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                Debug.WriteLine($"DeleteAsync: attempting to HARD DELETE client ID {id}");

                const string sql = "DELETE FROM users WHERE id = @Id AND role = 'client'";
                var result = await _dbContext.ExecuteCommandAsync(sql, new { Id = id });

                Debug.WriteLine($"DeleteAsync: {result} rows affected for ID {id}");

                return result > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeleteAsync Error: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Client>> GetAllIncludingDeletedAsync()
        {
            try
            {
                const string sql = @"
                    SELECT id, first_name, last_name, email, phone, age, photo, created_at, is_active
                    FROM users
                    WHERE role = 'client'";

                var clients = await _dbContext.ExecuteQueryListAsync<ClientDb>(sql);

                Debug.WriteLine($"GetAllIncludingDeletedAsync: loaded {clients.Count()} clients (including deleted)");

                return MapToClientList(clients);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetAllIncludingDeletedAsync Error: {ex.Message}");
                return new List<Client>();
            }
        }

        #region Private Helper Methods

        private async Task<bool> EmailExistsAsync(string email)
        {
            const string sql = "SELECT COUNT(*) FROM users WHERE email = @Email AND is_active = 1";
            var count = await _dbContext.ExecuteScalarAsync<int>(sql, new { Email = email });
            return count > 0;
        }

        private async Task<bool> EmailExistsForOtherClientAsync(string email, int clientId)
        {
            const string sql = "SELECT COUNT(*) FROM users WHERE email = @Email AND id != @Id AND is_active = 1";
            var count = await _dbContext.ExecuteScalarAsync<int>(sql, new { Email = email, Id = clientId });
            return count > 0;
        }

        private async Task LogDeletionStatus(int id)
        {
            const string sql = "SELECT is_active FROM users WHERE id = @Id";
            var isActive = await _dbContext.ExecuteScalarAsync<int>(sql, new { Id = id });
            Debug.WriteLine($"DeleteAsync: after update, is_active = {isActive} for ID {id}");
        }


        private List<Client> MapToClientList(IEnumerable<ClientDb> clientDbs)
        {
            return clientDbs.Select(c => new Client
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Phone = c.Phone,
                Age = c.Age,
                Photo = c.Photo,
                CreatedAt = c.CreatedAt,
                IsActive = c.IsActive
            }).ToList();
        }

        #endregion

        private class ClientDb
        {
            public int Id { get; set; }
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Phone { get; set; }
            public int? Age { get; set; }
            public byte[]? Photo { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool IsActive { get; set; }
        }
    }
}