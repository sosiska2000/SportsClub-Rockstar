using System.Text.Json.Serialization;

namespace RockstarAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;

    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int? Age { get; set; }
    public byte[]? Photo { get; set; }
    public string Role { get; set; } = "client";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}