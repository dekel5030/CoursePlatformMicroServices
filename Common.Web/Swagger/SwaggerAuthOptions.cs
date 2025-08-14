namespace Common.Web.Swagger;

public sealed class SwaggerAuthOptions
{
    public bool Enabled { get; init; } = false;
    public string SchemeName { get; init; } = "Bearer";
    public string HeaderName { get; init; } = "Authorization";
    public string Scheme { get; init; } = "bearer";
    public string? BearerFormat { get; init; } = "JWT";
    public string In { get; init; } = "Header"; // Header/Query/Cookie
    public string? Description { get; init; }
}