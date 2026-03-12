using System;

namespace Rockstar.Admin.WPF.Models
{
    public class Service
    {
        public int Id { get; set; }
        public int DirectionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int SessionsCount { get; set; } = 1;
        public int? DurationMinutes { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string DisplayName => $"{Name} - {Price:C}";
        public string DurationDisplay => DurationMinutes.HasValue ? $"{DurationMinutes} мин" : "—";
        public string SessionsDisplay => SessionsCount > 1 ? $"{SessionsCount} занятий" : "Разовое";
    }
}