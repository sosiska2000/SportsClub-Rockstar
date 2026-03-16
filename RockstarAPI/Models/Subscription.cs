namespace RockstarAPI.Models;

public class Subscription
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? DirectionId { get; set; }
    public decimal Price { get; set; }
    public int SessionsCount { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Direction? Direction { get; set; }
}