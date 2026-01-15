using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Contracts.Courses;
using Courses.Api.Contracts.Shared;
using Courses.Api.Extensions;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Application.Courses.Queries.GetCourses;
using Courses.Application.Shared.Dtos;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Courses;

internal sealed class GetCourses : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("courses", async (
            [AsParameters] PagedQueryDto pagedQuery,
            IMediator mediator,
            LinkProvider linksProvider,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            bool includeLinks = context.Items[MediaTypes.IncludeLinks] as bool? ?? false;

            var query = new GetCoursesQuery(pagedQuery);
            Result<PagedResponseDto<CourseSummaryDto>> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto.ToApiContract(linksProvider, pagedQuery, includeLinks)),
                CustomResults.Problem);
        })
        .AddEndpointFilter<HateoasFilter>()
        .WithMetadata<PagedResponse<CourseSummaryResponse>>(
            nameof(GetCourses),
            tag: Tags.Courses,
            summary: "Gets a paginated list of courses.");
    }
}

internal static class MediaTypes
{
    public const string IncludeLinks = "IncludeLinks";
    internal static class Application
    {
        public const string HateoasJson = "application/vnd.courseplatform.hateoas+json";
    }
}

internal sealed class HateoasFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        string acceptHeader = context.HttpContext.Request.Headers.Accept.ToString();

        bool prefersHateoas = acceptHeader.Contains(MediaTypes.Application.HateoasJson, StringComparison.OrdinalIgnoreCase);

        context.HttpContext.Items[MediaTypes.IncludeLinks] = prefersHateoas;

        object? result = await next(context);

        if (prefersHateoas && result is IResult)
        {
            context.HttpContext.Response.ContentType = MediaTypes.Application.HateoasJson;
        }

        return result;
    }
}