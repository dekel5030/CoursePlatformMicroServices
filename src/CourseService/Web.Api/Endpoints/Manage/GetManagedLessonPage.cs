using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Features.LessonPage;
using Courses.Application.Features.Management.ManagedLessonPage;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Manage;

internal sealed class GetManagedLessonPage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("manage/lessons/{lessonId:Guid}", async (
            Guid lessonId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new ManagedLessonPageQuery(lessonId);

            Result<LessonPageDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto),
                CustomResults.Problem);
        })
        .WithName(nameof(GetManagedLessonPage))
        .WithMetadata<LessonPageDto>(
            nameof(GetManagedLessonPage),
            tag: Tags.Lessons,
            summary: "Gets a lesson page for management by the instructor.")
        .RequireAuthorization();
    }
}
