using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace App.Rockstar.Admin.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        [JsonIgnore] // Не сериализуем пароль в логах
        public string Password { get; set; } = string.Empty;
    }
}