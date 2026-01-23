using Courses.Api.Endpoints.Contracts.Shared;
using Courses.Application.Abstractions.LinkProvider;

namespace Courses.Api.Endpoints.Contracts.Categories;

internal sealed record CategoryResponse(
    Guid Id,
    string Name,
    string Slug,
    IReadOnlyCollection<LinkDto> Links) : ILinksResponse;
