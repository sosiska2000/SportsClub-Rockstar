using System;

namespace Rockstar.Admin.WPF.Models
{
    public class Trainer
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public byte[]? Photo { get; set; }
        public int Experience { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        public string FullName => $"{LastName} {FirstName}";

        public string DirectionDisplayName => Direction switch
        {
            "yoga" => "Йога",
            "fitness" => "Фитнес",
            "climbing" => "Скалолазание",
            _ => Direction
        };

        public string PlainPassword { get; set; } = string.Empty;
    }
}