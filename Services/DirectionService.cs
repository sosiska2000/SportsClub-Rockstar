using Dapper;
using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Database;
using Rockstar.Admin.WPF.Services.Interfaces;
using System.Diagnostics;

namespace Rockstar.Admin.WPF.Services
{
    public class DirectionService : IDirectionService
    {
        private readonly MySqlDbContext _dbContext;

        public DirectionService(MySqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Direction>> GetAllAsync()
        {
            try
            {
                Debug.WriteLine("=== DirectionService.GetAllAsync ===");
                var sql = @"
                    SELECT id, name, name_key, description, is_active, created_at
                    FROM directions
                    WHERE is_active = TRUE
                    ORDER BY id";

                var directions = await _dbContext.ExecuteQueryListAsync<DirectionDb>(sql);
                Debug.WriteLine($"Found {directions.Count()} directions");

                return directions.Select(d => new Direction
                {
                    Id = d.id,
                    Name = d.name,
                    NameKey = d.name_key,
                    Description = d.description ?? string.Empty,
                    IsActive = d.is_active,
                    CreatedAt = d.created_at
                }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetAllAsync Error: {ex.Message}");
                return new List<Direction>();
            }
        }

        public async Task<Direction?> GetByIdAsync(int id)
        {
            try
            {
                var sql = @"
                    SELECT id, name, name_key, description, is_active, created_at
                    FROM directions
                    WHERE id = @Id AND is_active = TRUE";

                var direction = await _dbContext.ExecuteQueryAsync<DirectionDb>(sql, new { Id = id });

                if (direction == null) return null;

                return new Direction
                {
                    Id = direction.id,
                    Name = direction.name,
                    NameKey = direction.name_key,
                    Description = direction.description ?? string.Empty,
                    IsActive = direction.is_active,
                    CreatedAt = direction.created_at
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetByIdAsync Error: {ex.Message}");
                return null;
            }
        }

        public async Task<Direction?> GetByKeyAsync(string key)
        {
            try
            {
                var sql = @"
                    SELECT id, name, name_key, description, is_active, created_at
                    FROM directions
                    WHERE name_key = @Key AND is_active = TRUE";

                var direction = await _dbContext.ExecuteQueryAsync<DirectionDb>(sql, new { Key = key });

                if (direction == null) return null;

                return new Direction
                {
                    Id = direction.id,
                    Name = direction.name,
                    NameKey = direction.name_key,
                    Description = direction.description ?? string.Empty,
                    IsActive = direction.is_active,
                    CreatedAt = direction.created_at
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetByKeyAsync Error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateAsync(Direction direction)
        {
            try
            {
                Debug.WriteLine("=== DirectionService.CreateAsync ===");

                var sql = @"
                    INSERT INTO directions (name, name_key, description, is_active, created_at)
                    VALUES (@Name, @NameKey, @Description, TRUE, NOW())";

                var result = await _dbContext.ExecuteCommandAsync(sql, new
                {
                    direction.Name,
                    NameKey = direction.Name.ToLower().Replace(" ", "_"),
                    direction.Description
                });

                return result > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CreateAsync Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Direction direction)
        {
            try
            {
                Debug.WriteLine("=== DirectionService.UpdateAsync ===");

                var sql = @"
                    UPDATE directions 
                    SET name = @Name, 
                        name_key = @NameKey,
                        description = @Description,
                        updated_at = NOW()
                    WHERE id = @Id";

                var result = await _dbContext.ExecuteCommandAsync(sql, new
                {
                    direction.Id,
                    direction.Name,
                    NameKey = direction.Name.ToLower().Replace(" ", "_"),
                    direction.Description
                });

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
                Debug.WriteLine($"=== DirectionService.DeleteAsync ID: {id} ===");

                var sql = "UPDATE directions SET is_active = FALSE, updated_at = NOW() WHERE id = @Id";
                var result = await _dbContext.ExecuteCommandAsync(sql, new { Id = id });

                return result > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeleteAsync Error: {ex.Message}");
                return false;
            }
        }

        private class DirectionDb
        {
            public int id { get; set; }
            public string name { get; set; } = string.Empty;
            public string name_key { get; set; } = string.Empty;
            public string? description { get; set; }
            public bool is_active { get; set; }
            public DateTime created_at { get; set; }
        }
    }
}