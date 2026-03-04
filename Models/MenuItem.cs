using System.Windows.Input;

namespace Rockstar.Admin.WPF.Models
{
    public class MenuItem
    {
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public ICommand? Command { get; set; }
        public bool IsLogout { get; set; }
    }
}