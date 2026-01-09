using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
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
        app.MapGet("courses/{courseId:Guid}/lessons/{lessonId:Guid}", async (
            Guid courseId,
            Guid lessonId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetLessonByIdQuery(new CourseId(courseId), new LessonId(lessonId));

            Result<LessonDetailsDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithMetadata<LessonDetailsDto>(
            nameof(GetLessonById),
            tag: Tags.Lessons,
            summary: "Gets a lesson by its ID.");
    }
}