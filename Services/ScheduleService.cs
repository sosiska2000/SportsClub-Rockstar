using Dapper;
using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Database;
using Rockstar.Admin.WPF.Services.Interfaces;
using System.Diagnostics;

namespace Rockstar.Admin.WPF.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly MySqlDbContext _dbContext;

        public ScheduleService(MySqlDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        #region Расписание

        public async Task<List<Schedule>> GetGroupSchedulesAsync()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        s.id AS Id,
                        s.trainer_id AS TrainerId,
                        CONCAT(t.first_name, ' ', t.last_name) AS TrainerName,
                        s.direction_id AS DirectionId,
                        d.name AS DirectionName,
                        s.service_id AS ServiceId,
                        sv.name AS ServiceName,
                        s.datetime AS DateTime,
                        s.duration_minutes AS DurationMinutes,
                        s.max_participants AS MaxParticipants,
                        s.current_participants AS CurrentParticipants,
                        s.price AS Price,
                        s.is_group AS IsGroup,
                        s.is_active AS IsActive,
                        s.created_at AS CreatedAt
                    FROM schedule s
                    LEFT JOIN trainers t ON s.trainer_id = t.id
                    LEFT JOIN directions d ON s.direction_id = d.id
                    LEFT JOIN services sv ON s.service_id = sv.id
                    WHERE s.is_group = 1 AND s.is_active = 1
                    ORDER BY s.datetime ASC";

                var schedules = await _dbContext.ExecuteQueryListAsync<Schedule>(sql);
                Debug.WriteLine($"GetGroupSchedulesAsync: loaded {schedules.Count()} schedules");

                foreach (var s in schedules)
                {
                    Debug.WriteLine($"Schedule {s.Id}: {s.DirectionName} - {s.DateTime}");
                }

                return schedules.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetGroupSchedulesAsync Error: {ex.Message}");
                return new List<Schedule>();
            }
        }

        public async Task<Schedule?> GetScheduleByIdAsync(int id)
        {
            try
            {
                const string sql = @"
                    SELECT 
                        s.id AS Id,
                        s.trainer_id AS TrainerId,
                        CONCAT(t.first_name, ' ', t.last_name) AS TrainerName,
                        s.direction_id AS DirectionId,
                        d.name AS DirectionName,
                        s.service_id AS ServiceId,
                        sv.name AS ServiceName,
                        s.datetime AS DateTime,
                        s.duration_minutes AS DurationMinutes,
                        s.max_participants AS MaxParticipants,
                        s.current_participants AS CurrentParticipants,
                        s.price AS Price,
                        s.is_group AS IsGroup,
                        s.is_active AS IsActive,
                        s.created_at AS CreatedAt
                    FROM schedule s
                    LEFT JOIN trainers t ON s.trainer_id = t.id
                    LEFT JOIN directions d ON s.direction_id = d.id
                    LEFT JOIN services sv ON s.service_id = sv.id
                    WHERE s.id = @Id";

                var results = await _dbContext.ExecuteQueryListAsync<Schedule>(sql, new { Id = id });
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetScheduleByIdAsync Error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateScheduleAsync(Schedule schedule)
        {
            try
            {
                Debug.WriteLine($"CreateScheduleAsync: {schedule.DirectionName} - {schedule.DateTime}");

                const string sql = @"
                    INSERT INTO schedule 
                    (trainer_id, direction_id, service_id, datetime, duration_minutes, 
                     max_participants, current_participants, price, is_group, is_active, created_at)
                    VALUES 
                    (@TrainerId, @DirectionId, @ServiceId, @DateTime, @DurationMinutes,
                     @MaxParticipants, 0, @Price, 1, 1, NOW());
                    SELECT LAST_INSERT_ID();";

                var newId = await _dbContext.ExecuteScalarAsync<int>(sql, new
                {
                    schedule.TrainerId,
                    schedule.DirectionId,
                    schedule.ServiceId,
                    schedule.DateTime,
                    schedule.DurationMinutes,
                    schedule.MaxParticipants,
                    schedule.Price
                });

                Debug.WriteLine($"CreateScheduleAsync: new schedule ID = {newId}");
                return newId > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CreateScheduleAsync Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateScheduleAsync(Schedule schedule)
        {
            try
            {
                Debug.WriteLine($"UpdateScheduleAsync: ID={schedule.Id}");

                const string sql = @"
            UPDATE schedule 
            SET trainer_id = @TrainerId,
                direction_id = @DirectionId,
                service_id = @ServiceId,
                datetime = @DateTime,
                duration_minutes = @DurationMinutes,
                max_participants = @MaxParticipants,
                price = @Price,
                updated_at = NOW()
            WHERE id = @Id";

                var parameters = new
                {
                    schedule.Id,
                    TrainerId = schedule.TrainerId,  // может быть null
                    schedule.DirectionId,
                    ServiceId = schedule.ServiceId,   // может быть null
                    schedule.DateTime,
                    schedule.DurationMinutes,
                    schedule.MaxParticipants,
                    schedule.Price
                };

                var result = await _dbContext.ExecuteCommandAsync(sql, parameters);

                Debug.WriteLine($"UpdateScheduleAsync: {result} rows affected");
                return result > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UpdateScheduleAsync Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            try
            {
                Debug.WriteLine($"DeleteScheduleAsync: ID={id}");

                // Сначала удаляем связанные записи
                const string deleteEnrollments = "DELETE FROM enrollments WHERE schedule_id = @Id";
                await _dbContext.ExecuteCommandAsync(deleteEnrollments, new { Id = id });

                // Потом само занятие
                const string sql = "DELETE FROM schedule WHERE id = @Id";
                var result = await _dbContext.ExecuteCommandAsync(sql, new { Id = id });

                Debug.WriteLine($"DeleteScheduleAsync: {result} rows affected");
                return result > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeleteScheduleAsync Error: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Записи клиентов

        public async Task<List<Enrollment>> GetEnrollmentsByScheduleIdAsync(int scheduleId)
        {
            try
            {
                const string sql = @"
                    SELECT 
                        e.id AS Id,
                        e.user_id AS UserId,
                        CONCAT(u.first_name, ' ', u.last_name) AS UserName,
                        u.email AS UserEmail,
                        e.schedule_id AS ScheduleId,
                        e.enrolled_at AS EnrolledAt,
                        e.status AS Status,
                        e.created_at AS CreatedAt
                    FROM enrollments e
                    LEFT JOIN users u ON e.user_id = u.id
                    WHERE e.schedule_id = @ScheduleId
                    ORDER BY e.enrolled_at DESC";

                var enrollments = await _dbContext.ExecuteQueryListAsync<Enrollment>(sql, new { ScheduleId = scheduleId });
                Debug.WriteLine($"GetEnrollmentsByScheduleIdAsync: loaded {enrollments.Count()} enrollments");

                return enrollments.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetEnrollmentsByScheduleIdAsync Error: {ex.Message}");
                return new List<Enrollment>();
            }
        }

        public async Task<List<Client>> GetAvailableClientsAsync(int scheduleId)
        {
            try
            {
                const string sql = @"
                    SELECT 
                        u.id AS Id,
                        u.first_name AS FirstName,
                        u.last_name AS LastName,
                        u.email AS Email,
                        u.phone AS Phone,
                        u.age AS Age,
                        u.photo AS Photo,
                        u.created_at AS CreatedAt,
                        u.is_active AS IsActive
                    FROM users u
                    WHERE u.role = 'client' 
                        AND u.is_active = 1
                        AND u.id NOT IN (
                            SELECT user_id FROM enrollments 
                            WHERE schedule_id = @ScheduleId AND status = 'enrolled'
                        )
                    ORDER BY u.last_name, u.first_name";

                var clients = await _dbContext.ExecuteQueryListAsync<Client>(sql, new { ScheduleId = scheduleId });
                Debug.WriteLine($"GetAvailableClientsAsync: loaded {clients.Count()} available clients");

                return clients.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetAvailableClientsAsync Error: {ex.Message}");
                return new List<Client>();
            }
        }

        public async Task<bool> AddClientToScheduleAsync(int scheduleId, int userId)
        {
            try
            {
                Debug.WriteLine($"AddClientToScheduleAsync: schedule={scheduleId}, user={userId}");

                // Проверяем, не записан ли уже
                const string checkSql = "SELECT COUNT(*) FROM enrollments WHERE schedule_id = @ScheduleId AND user_id = @UserId";
                var exists = await _dbContext.ExecuteScalarAsync<int>(checkSql, new { ScheduleId = scheduleId, UserId = userId });

                if (exists > 0)
                {
                    Debug.WriteLine("Client already enrolled");
                    return false;
                }

                // Проверяем, есть ли места
                const string checkCapacity = "SELECT current_participants < max_participants FROM schedule WHERE id = @ScheduleId";
                var hasSpace = await _dbContext.ExecuteScalarAsync<bool>(checkCapacity, new { ScheduleId = scheduleId });

                if (!hasSpace)
                {
                    Debug.WriteLine("No available slots");
                    return false;
                }

                // Добавляем запись
                const string insertSql = @"
                    INSERT INTO enrollments (user_id, schedule_id, status, enrolled_at, created_at)
                    VALUES (@UserId, @ScheduleId, 'enrolled', NOW(), NOW())";

                var insertResult = await _dbContext.ExecuteCommandAsync(insertSql, new { ScheduleId = scheduleId, UserId = userId });

                if (insertResult > 0)
                {
                    // Обновляем счетчик
                    const string updateSql = "UPDATE schedule SET current_participants = current_participants + 1 WHERE id = @ScheduleId";
                    await _dbContext.ExecuteCommandAsync(updateSql, new { ScheduleId = scheduleId });

                    Debug.WriteLine($"AddClientToScheduleAsync: client added successfully");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AddClientToScheduleAsync Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveClientFromScheduleAsync(int enrollmentId)
        {
            try
            {
                Debug.WriteLine($"RemoveClientFromScheduleAsync: enrollment={enrollmentId}");

                // Получаем schedule_id перед удалением
                const string getScheduleSql = "SELECT schedule_id FROM enrollments WHERE id = @EnrollmentId";
                var scheduleId = await _dbContext.ExecuteScalarAsync<int>(getScheduleSql, new { EnrollmentId = enrollmentId });

                if (scheduleId == 0)
                {
                    Debug.WriteLine("Enrollment not found");
                    return false;
                }

                // Удаляем запись
                const string deleteSql = "DELETE FROM enrollments WHERE id = @EnrollmentId";
                var deleteResult = await _dbContext.ExecuteCommandAsync(deleteSql, new { EnrollmentId = enrollmentId });

                if (deleteResult > 0)
                {
                    // Обновляем счетчик
                    const string updateSql = "UPDATE schedule SET current_participants = current_participants - 1 WHERE id = @ScheduleId AND current_participants > 0";
                    await _dbContext.ExecuteCommandAsync(updateSql, new { ScheduleId = scheduleId });

                    Debug.WriteLine($"RemoveClientFromScheduleAsync: client removed successfully");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RemoveClientFromScheduleAsync Error: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Справочники

        public async Task<List<Direction>> GetDirectionsAsync()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        id AS Id,
                        name AS Name,
                        name_key AS NameKey,
                        description AS Description,
                        is_active AS IsActive,
                        created_at AS CreatedAt
                    FROM directions
                    WHERE is_active = 1
                    ORDER BY name";

                var directions = await _dbContext.ExecuteQueryListAsync<Direction>(sql);
                Debug.WriteLine($"GetDirectionsAsync: loaded {directions.Count()} directions");

                return directions.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetDirectionsAsync Error: {ex.Message}");
                return new List<Direction>();
            }
        }

        public async Task<List<Service>> GetServicesByDirectionAsync(int directionId)
        {
            try
            {
                const string sql = @"
                    SELECT 
                        id AS Id,
                        direction_id AS DirectionId,
                        name AS Name,
                        price AS Price,
                        sessions_count AS SessionsCount,
                        duration_minutes AS DurationMinutes,
                        description AS Description,
                        is_active AS IsActive,
                        created_at AS CreatedAt
                    FROM services
                    WHERE direction_id = @DirectionId AND is_active = 1
                    ORDER BY name";

                var services = await _dbContext.ExecuteQueryListAsync<Service>(sql, new { DirectionId = directionId });
                Debug.WriteLine($"GetServicesByDirectionAsync: loaded {services.Count()} services for direction {directionId}");

                return services.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetServicesByDirectionAsync Error: {ex.Message}");
                return new List<Service>();
            }
        }

        public async Task<List<Trainer>> GetTrainersAsync()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        id AS Id,
                        first_name AS FirstName,
                        last_name AS LastName,
                        direction_id AS DirectionId,
                        email AS Email,
                        password_hash AS PasswordHash,
                        photo AS Photo,
                        experience AS Experience,
                        description AS Description,
                        is_active AS IsActive,
                        created_at AS CreatedAt
                    FROM trainers
                    WHERE is_active = 1
                    ORDER BY last_name, first_name";

                var trainers = await _dbContext.ExecuteQueryListAsync<Trainer>(sql);

                // Заполняем Direction для обратной совместимости
                foreach (var trainer in trainers)
                {
                    if (trainer.DirectionId.HasValue)
                    {
                        trainer.Direction = trainer.DirectionId.Value switch
                        {
                            1 => "yoga",
                            2 => "fitness",
                            3 => "climbing",
                            _ => ""
                        };
                    }
                }

                Debug.WriteLine($"GetTrainersAsync: loaded {trainers.Count()} trainers");
                return trainers.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetTrainersAsync Error: {ex.Message}");
                return new List<Trainer>();
            }
        }

        #endregion
    }
}