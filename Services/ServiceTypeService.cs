using Dapper;
using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Database;
using Rockstar.Admin.WPF.Services.Interfaces;
using System.Diagnostics;

namespace Rockstar.Admin.WPF.Services
{
    public class ServiceTypeService : IServiceTypeService
    {
        private readonly MySqlDbContext _dbContext;

        public ServiceTypeService(MySqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ServiceType>> GetAllAsync()
        {
            try
            {
                Debug.WriteLine("=== ServiceTypeService.GetAllAsync ===");

                // В реальном проекте здесь должен быть запрос к таблице service_types
                // Создадим таблицу, если её нет
                await EnsureServiceTypesTableExistsAsync();

                var sql = @"
                    SELECT id, direction_id, name, description, default_duration as DefaultDuration, is_active
                    FROM service_types
                    WHERE is_active = TRUE
                    ORDER BY direction_id, id";

                var serviceTypes = await _dbContext.ExecuteQueryListAsync<ServiceTypeDb>(sql);
                Debug.WriteLine($"Found {serviceTypes.Count()} service types");

                return serviceTypes.Select(st => new ServiceType
                {
                    Id = st.id,
                    DirectionId = st.direction_id,
                    Name = st.name,
                    Description = st.description ?? string.Empty,
                    DefaultDuration = st.default_duration,
                    IsActive = st.is_active
                }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetAllAsync Error: {ex.Message}");
                return new List<ServiceType>();
            }
        }

        public async Task<List<ServiceType>> GetByDirectionIdAsync(int directionId)
        {
            try
            {
                Debug.WriteLine($"=== ServiceTypeService.GetByDirectionIdAsync({directionId}) ===");

                await EnsureServiceTypesTableExistsAsync();

                var sql = @"
                    SELECT id, direction_id, name, description, default_duration as DefaultDuration, is_active
                    FROM service_types
                    WHERE direction_id = @DirectionId AND is_active = TRUE
                    ORDER BY id";

                var serviceTypes = await _dbContext.ExecuteQueryListAsync<ServiceTypeDb>(sql, new { DirectionId = directionId });
                Debug.WriteLine($"Found {serviceTypes.Count()} service types for direction {directionId}");

                return serviceTypes.Select(st => new ServiceType
                {
                    Id = st.id,
                    DirectionId = st.direction_id,
                    Name = st.name,
                    Description = st.description ?? string.Empty,
                    DefaultDuration = st.default_duration,
                    IsActive = st.is_active
                }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetByDirectionIdAsync Error: {ex.Message}");
                return new List<ServiceType>();
            }
        }

        public async Task<ServiceType?> GetByIdAsync(int id)
        {
            try
            {
                await EnsureServiceTypesTableExistsAsync();

                var sql = @"
                    SELECT id, direction_id, name, description, default_duration as DefaultDuration, is_active
                    FROM service_types
                    WHERE id = @Id AND is_active = TRUE";

                var serviceType = await _dbContext.ExecuteQueryAsync<ServiceTypeDb>(sql, new { Id = id });

                if (serviceType == null) return null;

                return new ServiceType
                {
                    Id = serviceType.id,
                    DirectionId = serviceType.direction_id,
                    Name = serviceType.name,
                    Description = serviceType.description ?? string.Empty,
                    DefaultDuration = serviceType.default_duration,
                    IsActive = serviceType.is_active
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetByIdAsync Error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateAsync(ServiceType serviceType)
        {
            try
            {
                Debug.WriteLine("=== ServiceTypeService.CreateAsync ===");

                await EnsureServiceTypesTableExistsAsync();

                var sql = @"
                    INSERT INTO service_types (direction_id, name, description, default_duration, is_active)
                    VALUES (@DirectionId, @Name, @Description, @DefaultDuration, TRUE)";

                var result = await _dbContext.ExecuteCommandAsync(sql, new
                {
                    serviceType.DirectionId,
                    serviceType.Name,
                    serviceType.Description,
                    serviceType.DefaultDuration
                });

                return result > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CreateAsync Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(ServiceType serviceType)
        {
            try
            {
                Debug.WriteLine("=== ServiceTypeService.UpdateAsync ===");

                await EnsureServiceTypesTableExistsAsync();

                var sql = @"
                    UPDATE service_types 
                    SET name = @Name,
                        description = @Description,
                        default_duration = @DefaultDuration,
                        updated_at = NOW()
                    WHERE id = @Id";

                var result = await _dbContext.ExecuteCommandAsync(sql, new
                {
                    serviceType.Id,
                    serviceType.Name,
                    serviceType.Description,
                    serviceType.DefaultDuration
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
                Debug.WriteLine($"=== ServiceTypeService.DeleteAsync ID: {id} ===");

                await EnsureServiceTypesTableExistsAsync();

                var sql = "UPDATE service_types SET is_active = FALSE, updated_at = NOW() WHERE id = @Id";
                var result = await _dbContext.ExecuteCommandAsync(sql, new { Id = id });

                return result > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeleteAsync Error: {ex.Message}");
                return false;
            }
        }

        private async Task EnsureServiceTypesTableExistsAsync()
        {
            try
            {
                var sql = @"
                    CREATE TABLE IF NOT EXISTS service_types (
                        id INT PRIMARY KEY AUTO_INCREMENT,
                        direction_id INT NOT NULL,
                        name VARCHAR(255) NOT NULL,
                        description TEXT,
                        default_duration INT DEFAULT 60,
                        is_active BOOLEAN DEFAULT TRUE,
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                        FOREIGN KEY (direction_id) REFERENCES directions(id) ON DELETE CASCADE
                    )";

                await _dbContext.ExecuteCommandAsync(sql);
                Debug.WriteLine("Ensured service_types table exists");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error ensuring table exists: {ex.Message}");
            }
        }

        private class ServiceTypeDb
        {
            public int id { get; set; }
            public int direction_id { get; set; }
            public string name { get; set; } = string.Empty;
            public string? description { get; set; }
            public int default_duration { get; set; }
            public bool is_active { get; set; }
        }
    }
}