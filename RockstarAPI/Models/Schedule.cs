namespace RockstarAPI.Models;

public class Schedule
{
    public int Id { get; set; }
    public int? TrainerId { get; set; }
    public int DirectionId { get; set; }
    public int? ServiceId { get; set; }
    public DateTime DateTime { get; set; }
    public int DurationMinutes { get; set; }
    public int MaxParticipants { get; set; } = 20;
    public int CurrentParticipants { get; set; }
    public decimal Price { get; set; }
    public bool IsGroup { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Trainer? Trainer { get; set; }
    public Direction? Direction { get; set; }
    public Service? Service { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}