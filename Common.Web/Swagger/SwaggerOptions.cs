using System.ComponentModel.DataAnnotations;

namespace Common.Web.Swagger;

public sealed class SwaggerOptions
{
    public const string SectionName = "Swagger";

    [Required]
    public string Title { get; init; } = default!;

    [Required]
    public string Version { get; init; } = default!;

    public string? Description { get; init; }
    public bool EnableUI { get; init; } = true;
    public string RoutePrefix { get; init; } = "swagger";
    public SwaggerAuthOptions Auth { get; init; } = new();
}