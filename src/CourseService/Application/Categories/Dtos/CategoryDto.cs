using Courses.Application.Services.LinkProvider.Abstractions;

namespace Courses.Application.Categories.Dtos;

public record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    List<LinkDto>? Links = null
);
