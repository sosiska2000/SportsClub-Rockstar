using System;

namespace Rockstar.Admin.WPF.Models
{
    public class Trainer
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int? DirectionId { get; set; }
        public string DirectionName { get; set; } = string.Empty;
        public string DirectionKey { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public byte[]? Photo { get; set; }
        public int Experience { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public string FullName => $"{LastName} {FirstName}".Trim();
        public string DirectionDisplayName => DirectionName ?? "Не указано";
        public string Direction
        {
            get => DirectionKey;
            set => DirectionKey = value;
        }
        public string DirectionColor => DirectionKey switch
        {
            "yoga" => "#FF9F4D",
            "fitness" => "#4CAF50",
            "climbing" => "#2196F3",
            _ => "#9C27B0"
        };
        public string PlainPassword { get; set; } = string.Empty;
    }
}