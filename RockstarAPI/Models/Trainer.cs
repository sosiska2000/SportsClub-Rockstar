using System.Text.Json.Serialization;

namespace RockstarAPI.Models;

public class Trainer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int? DirectionId { get; set; }
    public string? Email { get; set; }

    [JsonIgnore]
    public string? PasswordHash { get; set; }

    public byte[]? Photo { get; set; }
    public int Experience { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Direction? Direction { get; set; }
}