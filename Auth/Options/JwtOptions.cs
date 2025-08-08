using System.ComponentModel.DataAnnotations;

namespace Common.Auth.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public required string Key { get; set; }

    [Required]
    public required string Issuer { get; set; }

    [Required]
    public required string Audience { get; set; }

    [Range(1, 1440)]
    public int ExpirationMinutes { get; set; } = 60;
}