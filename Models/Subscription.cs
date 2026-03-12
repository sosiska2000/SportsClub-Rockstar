using System;

namespace Rockstar.Admin.WPF.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? DirectionId { get; set; }
        public string DirectionName { get; set; } = string.Empty;
        public string DirectionKey { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int SessionsCount { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Для отображения в UI
        public string PriceDisplay => $"{Price:C}";
        public string SessionsDisplay => SessionsCount > 1 ? $"{SessionsCount} занятий" : "Разовое";
        public string DirectionDisplay => DirectionName ?? "Без направления";

        // Цвет для карточки в зависимости от направления
        public string DirectionColor => DirectionKey switch
        {
            "yoga" => "#FF9F4D", // Оранжевый для йоги
            "fitness" => "#4CAF50", // Зеленый для фитнеса
            "climbing" => "#2196F3", // Синий для скалолазания
            _ => "#9C27B0" // Фиолетовый для остальных
        };
    }
}