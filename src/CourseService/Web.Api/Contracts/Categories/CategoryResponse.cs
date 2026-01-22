using Courses.Api.Contracts.Shared;
using Courses.Api.Infrastructure.LinkProvider;

namespace Courses.Api.Contracts.Categories;

internal sealed record CategoryResponse(
    Guid Id,
    string Name,
    string Slug,
    IReadOnlyCollection<LinkDto> Links) : ILinksResponse;
