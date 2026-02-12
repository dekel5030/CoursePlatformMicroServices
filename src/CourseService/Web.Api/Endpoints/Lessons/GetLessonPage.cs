using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Lessons.Queries.GetLessons;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Lessons;

internal sealed class GetLessonPage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("lessons/{lessonId:Guid}", async (
            Guid lessonId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetLessonsQuery(new LessonFilter(Ids: [lessonId]));
            Result<IReadOnlyList<LessonDto>> result = await mediator.Send(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithMetadata<LessonDto>(
            nameof(GetLessonPage),
            tag: Tags.Lessons,
            summary: "Gets a lesson by its ID.");
    }
}
