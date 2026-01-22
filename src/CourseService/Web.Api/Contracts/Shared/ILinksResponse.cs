using Courses.Application.Abstractions.Hateoas;

namespace Courses.Api.Contracts.Shared;

internal interface ILinksResponse
{
    IReadOnlyCollection<LinkDto> Links { get; init; }
}
