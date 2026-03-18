using System;

namespace Rockstar.Admin.WPF.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int? Age { get; set; }
        public byte[]? Photo { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; } = string.Empty;

        public string FullName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName))
                    return "Имя не указано";
                return $"{FirstName} {LastName}".Trim();
            }
        }
    }
}