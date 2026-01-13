using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Contracts.Courses;
using Courses.Api.Extensions;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Application.Courses.Queries.GetById;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Courses;

internal sealed class GetCourseById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("courses/{id:Guid}", async (
            Guid id,
            IMediator mediator,
            HttpContext context,
            LinkGenerator linkGenerator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCourseByIdQuery(new CourseId(id));

            Result<CourseDetailsDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto.ToApiContract()),
                CustomResults.Problem);
        })
        .WithMetadata<CourseDetailsResponse>(
            nameof(GetCourseById),
            tag: Tags.Courses,
            summary: "Gets a course by its ID.");
    }
}

internal sealed record LinkDto
{
    public required string Href { get; init; }
    public required string Rel { get; init; }
    public required string Method { get; set; }
}

internal sealed class LinkService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LinkGenerator _linkGenerator;

    public LinkService(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
    {
        _httpContextAccessor = httpContextAccessor;
        _linkGenerator = linkGenerator;
    }

    public LinkDto Create(string endpointName, string rel, string method, object? values = null)
    {
        HttpContext httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HTTP context is not available.");

        string? href = _linkGenerator.GetUriByName(
            httpContext,
            endpointName,
            values);

        return new LinkDto
        {
            Href = href ?? throw new InvalidOperationException($"Could not generate URL for endpoint '{endpointName}'."),
            Rel = rel,
            Method = method
        };
    }
}
//internal static class LinkServiceExtensions
//{

//}
