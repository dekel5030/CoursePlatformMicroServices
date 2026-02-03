using Courses.Application.Services.LinkProvider.Abstractions;

namespace Courses.Application.Courses.Dtos;

public record ModuleDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }

    public required List<LinkDto> Links { get; init; }
}
