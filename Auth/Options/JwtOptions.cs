using System.ComponentModel.DataAnnotations;

namespace Common.Auth.Options;


public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required(ErrorMessage = "Jwt:Key is required.")]
    public required string Key { get; init; }

    [Required(ErrorMessage = "Jwt:Issuer is required.")]
    public required string Issuer { get; init; }

    [Required(ErrorMessage = "Jwt:Audience is required.")]
    public required string Audience { get; init; }

    [Range(1, 24 * 60, ErrorMessage = "Jwt:ExpirationMinutes must be between 1 and 1440.")]
    public int ExpirationMinutes { get; init; } = 60;
}