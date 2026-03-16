using Rockstar.Admin.WPF.Models;
using Rockstar.Admin.WPF.Services.Interfaces;

namespace Rockstar.Admin.WPF.Services;

public class ApiScheduleService : IScheduleService
{
    private readonly IApiService _apiService;

    public ApiScheduleService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<Schedule>> GetGroupSchedulesAsync()
    {
        return await _apiService.GetAsync<List<Schedule>>("schedule/group") ?? new List<Schedule>();
    }

    public async Task<Schedule?> GetScheduleByIdAsync(int id)
    {
        return await _apiService.GetAsync<Schedule>($"schedule/{id}");
    }

    public async Task<bool> CreateScheduleAsync(Schedule schedule)
    {
        var result = await _apiService.PostAsync<Schedule>("schedule", schedule);
        return result != null;
    }

    public async Task<bool> UpdateScheduleAsync(Schedule schedule)
    {
        var result = await _apiService.PutAsync<Schedule>($"schedule/{schedule.Id}", schedule);
        return result != null;
    }

    public async Task<bool> DeleteScheduleAsync(int id)
    {
        return await _apiService.DeleteAsync($"schedule/{id}");
    }

    public async Task<List<Enrollment>> GetEnrollmentsByScheduleIdAsync(int scheduleId)
    {
        return await _apiService.GetAsync<List<Enrollment>>($"schedule/{scheduleId}/enrollments")
               ?? new List<Enrollment>();
    }

    public async Task<List<Client>> GetAvailableClientsAsync(int scheduleId)
    {
        return await _apiService.GetAsync<List<Client>>($"schedule/{scheduleId}/available-clients")
               ?? new List<Client>();
    }

    public async Task<bool> AddClientToScheduleAsync(int scheduleId, int userId)
    {
        var data = new { UserId = userId };
        var result = await _apiService.PostAsync<object>($"schedule/{scheduleId}/enroll", data);
        return result != null;
    }

    public async Task<bool> RemoveClientFromScheduleAsync(int enrollmentId)
    {
        return await _apiService.DeleteAsync($"enrollments/{enrollmentId}");
    }

    public async Task<List<Direction>> GetDirectionsAsync()
    {
        return await _apiService.GetAsync<List<Direction>>("directions") ?? new List<Direction>();
    }

    public async Task<List<Service>> GetServicesByDirectionAsync(int directionId)
    {
        return await _apiService.GetAsync<List<Service>>($"directions/{directionId}/services")
               ?? new List<Service>();
    }

    public async Task<List<Trainer>> GetTrainersAsync()
    {
        return await _apiService.GetAsync<List<Trainer>>("trainers") ?? new List<Trainer>();
    }
}