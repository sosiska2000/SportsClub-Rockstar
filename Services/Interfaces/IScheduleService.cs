using Rockstar.Admin.WPF.Models;

namespace Rockstar.Admin.WPF.Services.Interfaces
{
    public interface IScheduleService
    {
        // Расписание
        Task<List<Schedule>> GetGroupSchedulesAsync();
        Task<Schedule?> GetScheduleByIdAsync(int id);
        Task<bool> CreateScheduleAsync(Schedule schedule);
        Task<bool> UpdateScheduleAsync(Schedule schedule);
        Task<bool> DeleteScheduleAsync(int id);

        // Записи клиентов
        Task<List<Enrollment>> GetEnrollmentsByScheduleIdAsync(int scheduleId);
        Task<List<Client>> GetAvailableClientsAsync(int scheduleId);
        Task<bool> AddClientToScheduleAsync(int scheduleId, int userId);
        Task<bool> RemoveClientFromScheduleAsync(int enrollmentId);

        // Справочники
        Task<List<Direction>> GetDirectionsAsync();
        Task<List<Service>> GetServicesByDirectionAsync(int directionId);
        Task<List<Trainer>> GetTrainersAsync();
    }
}