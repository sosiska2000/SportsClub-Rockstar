using Dapper;
using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Database;
using Rockstar.Admin.WPF.Services.Interfaces;
using System.Diagnostics;

namespace Rockstar.Admin.WPF.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly MySqlDbContext _dbContext;

        public SubscriptionService(MySqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Subscription>> GetAllAsync()
        {
            try
            {
                Debug.WriteLine("=== SubscriptionService.GetAllAsync ===");
                var sql = @"
                    SELECT s.id, s.name, s.direction_id, s.price, s.sessions_count, 
                           s.description, s.is_active, s.created_at,
                           d.name as direction_name, d.name_key as direction_key
                    FROM subscriptions s
                    LEFT JOIN directions d ON s.direction_id = d.id
                    WHERE s.is_active = TRUE
                    ORDER BY s.direction_id, s.id";

                var subscriptions = await _dbContext.ExecuteQueryListAsync<SubscriptionDb>(sql);
                Debug.WriteLine($"Found {subscriptions.Count()} subscriptions");

                return subscriptions.Select(s => new Subscription
                {
                    Id = s.id,
                    Name = s.name,
                    DirectionId = s.direction_id,
                    DirectionName = s.direction_name ?? string.Empty,
                    DirectionKey = s.direction_key ?? string.Empty,
                    Price = s.price,
                    SessionsCount = s.sessions_count,
                    Description = s.description ?? string.Empty,
                    IsActive = s.is_active,
                    CreatedAt = s.created_at
                }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetAllAsync Error: {ex.Message}");
                return new List<Subscription>();
            }
        }

        public async Task<List<Subscription>> GetByDirectionIdAsync(int directionId)
        {
            try
            {
                Debug.WriteLine($"=== SubscriptionService.GetByDirectionIdAsync({directionId}) ===");
                var sql = @"
                    SELECT s.id, s.name, s.direction_id, s.price, s.sessions_count, 
                           s.description, s.is_active, s.created_at,
                           d.name as direction_name, d.name_key as direction_key
                    FROM subscriptions s
                    LEFT JOIN directions d ON s.direction_id = d.id
                    WHERE s.direction_id = @DirectionId AND s.is_active = TRUE
                    ORDER BY s.id";

                var subscriptions = await _dbContext.ExecuteQueryListAsync<SubscriptionDb>(sql, new { DirectionId = directionId });
                Debug.WriteLine($"Found {subscriptions.Count()} subscriptions for direction {directionId}");

                return subscriptions.Select(s => new Subscription
                {
                    Id = s.id,
                    Name = s.name,
                    DirectionId = s.direction_id,
                    DirectionName = s.direction_name ?? string.Empty,
                    DirectionKey = s.direction_key ?? string.Empty,
                    Price = s.price,
                    SessionsCount = s.sessions_count,
                    Description = s.description ?? string.Empty,
                    IsActive = s.is_active,
                    CreatedAt = s.created_at
                }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetByDirectionIdAsync Error: {ex.Message}");
                return new List<Subscription>();
            }
        }

        public async Task<Subscription?> GetByIdAsync(int id)
        {
            try
            {
                var sql = @"
                    SELECT s.id, s.name, s.direction_id, s.price, s.sessions_count, 
                           s.description, s.is_active, s.created_at,
                           d.name as direction_name, d.name_key as direction_key
                    FROM subscriptions s
                    LEFT JOIN directions d ON s.direction_id = d.id
                    WHERE s.id = @Id AND s.is_active = TRUE";

                var subscription = await _dbContext.ExecuteQueryAsync<SubscriptionDb>(sql, new { Id = id });

                if (subscription == null) return null;

                return new Subscription
                {
                    Id = subscription.id,
                    Name = subscription.name,
                    DirectionId = subscription.direction_id,
                    DirectionName = subscription.direction_name ?? string.Empty,
                    DirectionKey = subscription.direction_key ?? string.Empty,
                    Price = subscription.price,
                    SessionsCount = subscription.sessions_count,
                    Description = subscription.description ?? string.Empty,
                    IsActive = subscription.is_active,
                    CreatedAt = subscription.created_at
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetByIdAsync Error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateAsync(Subscription subscription)
        {
            try
            {
                Debug.WriteLine("=== SubscriptionService.CreateAsync ===");

                var sql = @"
                    INSERT INTO subscriptions (name, direction_id, price, sessions_count, description, is_active, created_at)
                    VALUES (@Name, @DirectionId, @Price, @SessionsCount, @Description, TRUE, NOW())";

                var result = await _dbContext.ExecuteCommandAsync(sql, new
                {
                    subscription.Name,
                    subscription.DirectionId,
                    subscription.Price,
                    subscription.SessionsCount,
                    subscription.Description
                });

                return result > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CreateAsync Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Subscription subscription)
        {
            try
            {
                Debug.WriteLine("=== SubscriptionService.UpdateAsync ===");

                var sql = @"
                    UPDATE subscriptions 
                    SET name = @Name,
                        direction_id = @DirectionId,
                        price = @Price,
                        sessions_count = @SessionsCount,
                        description = @Description,
                        updated_at = NOW()
                    WHERE id = @Id";

                var result = await _dbContext.ExecuteCommandAsync(sql, new
                {
                    subscription.Id,
                    subscription.Name,
                    subscription.DirectionId,
                    subscription.Price,
                    subscription.SessionsCount,
                    subscription.Description
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
                Debug.WriteLine($"=== SubscriptionService.DeleteAsync ID: {id} ===");

                var sql = "UPDATE subscriptions SET is_active = FALSE, updated_at = NOW() WHERE id = @Id";
                var result = await _dbContext.ExecuteCommandAsync(sql, new { Id = id });

                return result > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeleteAsync Error: {ex.Message}");
                return false;
            }
        }

        private class SubscriptionDb
        {
            public int id { get; set; }
            public string name { get; set; } = string.Empty;
            public int? direction_id { get; set; }
            public string? direction_name { get; set; }
            public string? direction_key { get; set; }
            public decimal price { get; set; }
            public int sessions_count { get; set; }
            public string? description { get; set; }
            public bool is_active { get; set; }
            public DateTime created_at { get; set; }
        }
    }
}