using System;
using System.Collections.ObjectModel;

namespace Rockstar.Admin.WPF.Models
{
    public class Direction
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NameKey { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ObservableCollection<Service> Services { get; set; } = new();
    }
}