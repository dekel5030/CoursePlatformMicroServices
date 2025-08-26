using System.ComponentModel.DataAnnotations;

namespace AuthService.Settings;

public class AuthDbSettings
{
    public const string SectionName = "AuthDb";

    [Required]
    public required string Host { get; set; }

    [Range(1, 65535)]
    public required int Port { get; set; }

    [Required]
    public required string Database { get; set; }

    [Required]
    public required string Username { get; set; }

    [Required]
    public required string Password { get; set; }

    public string BuildConnectionString()
    {
        return $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password}";
    }
}