using Dapper;
using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Database;
using Rockstar.Admin.WPF.Services.Interfaces;
using System.Diagnostics;

namespace Rockstar.Admin.WPF.Services
{
    public class ServiceService : IServiceService
    {
        private readonly MySqlDbContext _dbContext;

        public ServiceService(MySqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Service>> GetAllAsync()
        {
            try
            {
                Debug.WriteLine("=== ServiceService.GetAllAsync ===");
                var sql = @"
                    SELECT id, direction_id, name, price, sessions_count, 
                           duration_minutes, description, is_active, created_at
                    FROM services
                    WHERE is_active = TRUE
                    ORDER BY direction_id, id";

                var services = await _dbContext.ExecuteQueryListAsync<ServiceDb>(sql);
                Debug.WriteLine($"Found {services.Count()} services");

                return services.Select(s => new Service
                {
                    Id = s.id,
                    DirectionId = s.direction_id,
                    Name = s.name,
                    Price = s.price,
                    SessionsCount = s.sessions_count,
                    DurationMinutes = s.duration_minutes,
                    Description = s.description ?? string.Empty,
                    IsActive = s.is_active,
                    CreatedAt = s.created_at
                }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetAllAsync Error: {ex.Message}");
                return new List<Service>();
            }
        }

        public async Task<List<Service>> GetByDirectionIdAsync(int directionId)
        {
            try
            {
                Debug.WriteLine($"=== ServiceService.GetByDirectionIdAsync({directionId}) ===");
                var sql = @"
                    SELECT id, direction_id, name, price, sessions_count, 
                           duration_minutes, description, is_active, created_at
                    FROM services
                    WHERE direction_id = @DirectionId AND is_active = TRUE
                    ORDER BY id";

                var services = await _dbContext.ExecuteQueryListAsync<ServiceDb>(sql, new { DirectionId = directionId });
                Debug.WriteLine($"Found {services.Count()} services for direction {directionId}");

                return services.Select(s => new Service
                {
                    Id = s.id,
                    DirectionId = s.direction_id,
                    Name = s.name,
                    Price = s.price,
                    SessionsCount = s.sessions_count,
                    DurationMinutes = s.duration_minutes,
                    Description = s.description ?? string.Empty,
                    IsActive = s.is_active,
                    CreatedAt = s.created_at
                }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetByDirectionIdAsync Error: {ex.Message}");
                return new List<Service>();
            }
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            try
            {
                var sql = @"
                    SELECT id, direction_id, name, price, sessions_count, 
                           duration_minutes, description, is_active, created_at
                    FROM services
                    WHERE id = @Id AND is_active = TRUE";

                var service = await _dbContext.ExecuteQueryAsync<ServiceDb>(sql, new { Id = id });

                if (service == null) return null;

                return new Service
                {
                    Id = service.id,
                    DirectionId = service.direction_id,
                    Name = service.name,
                    Price = service.price,
                    SessionsCount = service.sessions_count,
                    DurationMinutes = service.duration_minutes,
                    Description = service.description ?? string.Empty,
                    IsActive = service.is_active,
                    CreatedAt = service.created_at
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetByIdAsync Error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateAsync(Service service)
        {
            try
            {
                Debug.WriteLine("=== ServiceService.CreateAsync ===");

                var sql = @"
                    INSERT INTO services 
                    (direction_id, name, price, sessions_count, duration_minutes, description, is_active, created_at)
                    VALUES 
                    (@DirectionId, @Name, @Price, @SessionsCount, @DurationMinutes, @Description, TRUE, NOW())";

                var result = await _dbContext.ExecuteCommandAsync(sql, new
                {
                    service.DirectionId,
                    service.Name,
                    service.Price,
                    service.SessionsCount,
                    service.DurationMinutes,
                    service.Description
                });

                return result > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CreateAsync Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Service service)
        {
            try
            {
                Debug.WriteLine("=== ServiceService.UpdateAsync ===");

                var sql = @"
                    UPDATE services 
                    SET name = @Name, 
                        price = @Price,
                        sessions_count = @SessionsCount,
                        duration_minutes = @DurationMinutes,
                        description = @Description,
                        updated_at = NOW()
                    WHERE id = @Id";

                var result = await _dbContext.ExecuteCommandAsync(sql, new
                {
                    service.Id,
                    service.Name,
                    service.Price,
                    service.SessionsCount,
                    service.DurationMinutes,
                    service.Description
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
                Debug.WriteLine($"=== ServiceService.DeleteAsync ID: {id} ===");

                var sql = "UPDATE services SET is_active = FALSE, updated_at = NOW() WHERE id = @Id";
                var result = await _dbContext.ExecuteCommandAsync(sql, new { Id = id });

                return result > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeleteAsync Error: {ex.Message}");
                return false;
            }
        }

        private class ServiceDb
        {
            public int id { get; set; }
            public int direction_id { get; set; }
            public string name { get; set; } = string.Empty;
            public decimal price { get; set; }
            public int sessions_count { get; set; }
            public int? duration_minutes { get; set; }
            public string? description { get; set; }
            public bool is_active { get; set; }
            public DateTime created_at { get; set; }
        }
    }
}