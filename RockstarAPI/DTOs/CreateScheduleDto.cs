namespace RockstarAPI.DTOs;

public class CreateScheduleDto
{
    public int? TrainerId { get; set; }
    public int DirectionId { get; set; }
    public int? ServiceId { get; set; }
    public DateTime DateTime { get; set; }
    public int DurationMinutes { get; set; }
    public int MaxParticipants { get; set; } = 20;
    public decimal Price { get; set; }
    public bool IsGroup { get; set; } = true;
}

public class UpdateScheduleDto : CreateScheduleDto
{
    public int Id { get; set; }
}