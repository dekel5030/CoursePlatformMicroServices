using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Api.Infrastructure.Routing;
using Courses.Application.Lessons.Queries.Dtos;
using Courses.Application.Lessons.Queries.GetById;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Lessons;

internal sealed class GetLessonById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("courses/{courseId:CourseId}/lessons/{lessonId:LessonId}", async (
            CourseId courseId,
            LessonId lessonId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetLessonByIdQuery(courseId, lessonId);

            Result<LessonDetailsDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithMetadata<LessonDetailsDto>(
            nameof(GetLessonById),
            tag: Tags.Lessons,
            summary: "Gets a lesson by its ID.");
    }
}
