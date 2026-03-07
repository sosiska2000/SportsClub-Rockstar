using Dapper;
using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Database;
using Rockstar.Admin.WPF.Services.Interfaces;
using System.Diagnostics;
using System.Text;

namespace Rockstar.Admin.WPF.Services
{
    public class TrainerService : ITrainerService
    {
        private readonly MySqlDbContext _dbContext;

        public TrainerService(MySqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Trainer>> GetAllAsync()
        {
            try
            {
                Debug.WriteLine("=== TrainerService.GetAllAsync ===");
                var sql = @"
                    SELECT t.id, t.first_name, t.last_name, t.direction_id, t.email, 
                           t.password_hash, t.photo, t.experience, t.description, t.is_active, t.created_at,
                           d.name_key as direction
                    FROM trainers t
                    LEFT JOIN directions d ON t.direction_id = d.id
                    WHERE t.is_active = TRUE";

                var trainers = await _dbContext.ExecuteQueryListAsync<TrainerDb>(sql);
                Debug.WriteLine($"Found {trainers.Count()} trainers in database");

                return trainers.Select(t => MapToTrainer(t)).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetAllAsync Error: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                return new List<Trainer>();
            }
        }

        public async Task<Trainer?> GetByIdAsync(int id)
        {
            try
            {
                var sql = @"
                    SELECT t.id, t.first_name, t.last_name, t.direction_id, t.email, 
                           t.password_hash, t.photo, t.experience, t.description, t.is_active, t.created_at,
                           d.name_key as direction
                    FROM trainers t
                    LEFT JOIN directions d ON t.direction_id = d.id
                    WHERE t.id = @Id AND t.is_active = TRUE";

                var trainer = await _dbContext.ExecuteQueryAsync<TrainerDb>(sql, new { Id = id });
                return trainer == null ? null : MapToTrainer(trainer);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetByIdAsync Error: {ex.Message}");
                return null;
            }
        }

        public async Task<Trainer?> GetByEmailAsync(string email)
        {
            try
            {
                var sql = @"
                    SELECT t.id, t.first_name, t.last_name, t.direction_id, t.email, 
                           t.password_hash, t.photo, t.experience, t.description, t.is_active, t.created_at,
                           d.name_key as direction
                    FROM trainers t
                    LEFT JOIN directions d ON t.direction_id = d.id
                    WHERE t.email = @Email AND t.is_active = TRUE";

                var trainer = await _dbContext.ExecuteQueryAsync<TrainerDb>(sql, new { Email = email });
                return trainer == null ? null : MapToTrainer(trainer);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetByEmailAsync Error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateAsync(Trainer trainer)
        {
            try
            {
                Debug.WriteLine("=== TrainerService.CreateAsync START ===");

                // Проверяем, существует ли уже тренер с таким email
                var existingTrainer = await GetByEmailAsync(trainer.Email);
                if (existingTrainer != null)
                {
                    Debug.WriteLine($"ERROR: Trainer with email '{trainer.Email}' already exists!");
                    return false;
                }

                var directionId = await GetDirectionIdByKeyAsync(trainer.Direction);
                if (directionId == null)
                {
                    Debug.WriteLine("ERROR: Direction not found in database!");
                    return false;
                }

                // Хэшируем пароль (в реальном приложении используйте BCrypt)
                string passwordHash = HashPassword(trainer.PlainPassword);

                var sql = @"
                    INSERT INTO trainers 
                    (first_name, last_name, direction_id, email, password_hash, 
                     photo, experience, description, is_active, created_at)
                    VALUES 
                    (@FirstName, @LastName, @DirectionId, @Email, @PasswordHash, 
                     @Photo, @Experience, @Description, TRUE, NOW())";

                var parameters = new
                {
                    FirstName = trainer.FirstName,
                    LastName = trainer.LastName,
                    DirectionId = directionId,
                    Email = trainer.Email,
                    PasswordHash = passwordHash,
                    Photo = trainer.Photo,
                    Experience = trainer.Experience,
                    Description = trainer.Description ?? string.Empty
                };

                var result = await _dbContext.ExecuteCommandAsync(sql, parameters);
                return result > 0;
            }
            catch (MySqlConnector.MySqlException mysqlEx)
            {
                Debug.WriteLine($"MYSQL ERROR: {mysqlEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GENERAL ERROR: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Trainer trainer)
        {
            try
            {
                Debug.WriteLine("=== TrainerService.UpdateAsync ===");
                Debug.WriteLine($"Updating trainer ID: {trainer.Id}");

                // Получаем текущего тренера из БД
                var currentTrainer = await GetByIdAsync(trainer.Id);
                if (currentTrainer == null)
                {
                    Debug.WriteLine($"Trainer with ID {trainer.Id} not found!");
                    return false;
                }

                // Проверяем, не занят ли email другим тренером
                if (currentTrainer.Email != trainer.Email)
                {
                    var existingTrainer = await GetByEmailAsync(trainer.Email);
                    if (existingTrainer != null && existingTrainer.Id != trainer.Id)
                    {
                        Debug.WriteLine($"ERROR: Email '{trainer.Email}' is already used by another trainer!");
                        return false;
                    }
                }

                var directionId = await GetDirectionIdByKeyAsync(trainer.Direction);
                if (directionId == null)
                {
                    Debug.WriteLine("ERROR: Direction not found!");
                    return false;
                }

                // Формируем SQL запрос с учетом возможного обновления пароля
                string sql;
                object parameters;

                if (string.IsNullOrWhiteSpace(trainer.PlainPassword))
                {
                    // Если пароль не меняется - оставляем старый
                    sql = @"
                        UPDATE trainers 
                        SET first_name = @FirstName, 
                            last_name = @LastName, 
                            direction_id = @DirectionId,
                            email = @Email, 
                            photo = @Photo, 
                            experience = @Experience, 
                            description = @Description, 
                            updated_at = NOW()
                        WHERE id = @Id";

                    parameters = new
                    {
                        trainer.Id,
                        FirstName = trainer.FirstName,
                        LastName = trainer.LastName,
                        DirectionId = directionId,
                        Email = trainer.Email,
                        Photo = trainer.Photo,
                        Experience = trainer.Experience,
                        Description = trainer.Description
                    };
                }
                else
                {
                    // Если пароль меняется - хэшируем новый
                    string passwordHash = HashPassword(trainer.PlainPassword);

                    sql = @"
                        UPDATE trainers 
                        SET first_name = @FirstName, 
                            last_name = @LastName, 
                            direction_id = @DirectionId,
                            email = @Email, 
                            password_hash = @PasswordHash,
                            photo = @Photo, 
                            experience = @Experience, 
                            description = @Description, 
                            updated_at = NOW()
                        WHERE id = @Id";

                    parameters = new
                    {
                        trainer.Id,
                        FirstName = trainer.FirstName,
                        LastName = trainer.LastName,
                        DirectionId = directionId,
                        Email = trainer.Email,
                        PasswordHash = passwordHash,
                        Photo = trainer.Photo,
                        Experience = trainer.Experience,
                        Description = trainer.Description
                    };
                }

                Debug.WriteLine($"Executing update for trainer ID: {trainer.Id}");
                var result = await _dbContext.ExecuteCommandAsync(sql, parameters);
                Debug.WriteLine($"Update result: {result} rows affected");

                return result > 0;
            }
            catch (MySqlConnector.MySqlException mysqlEx)
            {
                Debug.WriteLine($"MYSQL ERROR: {mysqlEx.Message}");
                return false;
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
                Debug.WriteLine($"=== TrainerService.DeleteAsync ID: {id} ===");

                var sql = "UPDATE trainers SET is_active = FALSE, updated_at = NOW() WHERE id = @Id";
                var result = await _dbContext.ExecuteCommandAsync(sql, new { Id = id });

                Debug.WriteLine($"Delete result: {result} rows affected");
                return result > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeleteAsync Error: {ex.Message}");
                return false;
            }
        }

        public List<string> GetAvailableDirections()
        {
            return new List<string> { "yoga", "fitness", "climbing" };
        }

        private async Task<int?> GetDirectionIdByKeyAsync(string directionKey)
        {
            try
            {
                Debug.WriteLine($"Looking for direction: {directionKey}");
                var sql = "SELECT id FROM directions WHERE name_key = @Key LIMIT 1";
                var id = await _dbContext.ExecuteScalarAsync<int?>(sql, new { Key = directionKey });
                Debug.WriteLine($"Direction ID: {id}");
                return id;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetDirectionIdByKeyAsync Error: {ex.Message}");
                return null;
            }
        }

        // Простое хэширование пароля (для демонстрации)
        // В реальном приложении используйте BCrypt или другой надежный алгоритм
        private string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;

            // Это очень простой пример - НЕ ИСПОЛЬЗУЙТЕ В ПРОДАКШЕНЕ!
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private Trainer MapToTrainer(TrainerDb db)
        {
            return new Trainer
            {
                Id = db.id,
                FirstName = db.first_name ?? string.Empty,
                LastName = db.last_name ?? string.Empty,
                Direction = db.direction ?? "yoga",
                Email = db.email ?? string.Empty,
                PasswordHash = db.password_hash ?? string.Empty,
                Photo = db.photo,
                Experience = db.experience,
                Description = db.description ?? string.Empty,
                CreatedAt = db.created_at,
                IsActive = db.is_active
            };
        }

        private class TrainerDb
        {
            public int id { get; set; }
            public string first_name { get; set; } = string.Empty;
            public string last_name { get; set; } = string.Empty;
            public int? direction_id { get; set; }
            public string? direction { get; set; }
            public string? email { get; set; }
            public string? password_hash { get; set; }
            public byte[]? photo { get; set; }
            public int experience { get; set; }
            public string? description { get; set; }
            public bool is_active { get; set; }
            public DateTime created_at { get; set; }
        }
    }
}