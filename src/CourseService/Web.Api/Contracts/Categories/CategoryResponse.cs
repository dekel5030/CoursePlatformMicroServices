using Courses.Api.Contracts.Shared;
using Courses.Application.Abstractions.Hateoas;

namespace Courses.Api.Contracts.Categories;

internal sealed record CategoryResponse(
    Guid Id,
    string Name,
    string Slug,
    IReadOnlyCollection<LinkDto> Links) : ILinksResponse;
