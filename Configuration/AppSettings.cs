namespace Rockstar.Admin.WPF.Configuration
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; } = new();
        public ApiSettings ApiSettings { get; set; } = new();
        public AuthSettings AuthSettings { get; set; } = new();
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; } = string.Empty;
    }

    public class ApiSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
    }

    public class AuthSettings
    {
        public string JwtSecret { get; set; } = string.Empty;
        public int TokenExpirationHours { get; set; }
    }
}