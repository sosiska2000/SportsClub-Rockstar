using RockstarAPI.Models;

namespace RockstarAPI.Models;

public class Enrollment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ScheduleId { get; set; }
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "enrolled"; // enrolled, attended, cancelled, no_show
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User? User { get; set; }
    public Schedule? Schedule { get; set; }
}