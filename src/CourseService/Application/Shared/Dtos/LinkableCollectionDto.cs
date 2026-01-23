using Courses.Application.Abstractions.Hateoas;

namespace Courses.Application.Shared.Dtos;

public sealed record ResourceCollection<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required IReadOnlyList<LinkDto> Links { get; init; }
}

public interface IResource
{
    IReadOnlyList<LinkDto> Links { get; }
}
