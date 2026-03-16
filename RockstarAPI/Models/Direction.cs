namespace RockstarAPI.Models;

public class Direction
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameKey { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}