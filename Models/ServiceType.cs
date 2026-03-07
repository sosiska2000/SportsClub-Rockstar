namespace Rockstar.Admin.WPF.Models
{
    public class ServiceType
    {
        public int Id { get; set; }
        public int DirectionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DefaultDuration { get; set; } = 60;
        public bool IsActive { get; set; } = true;
        public string DisplayName => $"{Name} ({DefaultDuration} мин)";
    }
}