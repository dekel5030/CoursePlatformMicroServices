namespace Courses.Application.Abstractions.LinkProvider;

public sealed record LinkDto
{
    public required string Href { get; init; }
    public required string Rel { get; init; }
    public required string Method { get; set; }
}
