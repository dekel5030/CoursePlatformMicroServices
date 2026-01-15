using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Contracts.Lessons;
using Courses.Api.Extensions;
using Courses.Api.Infrastructure.LinkProvider;
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
            LinkProvider linkProvider,
            CancellationToken cancellationToken) =>
        {
            var courseIdObj = new CourseId(courseId);
            var lessonIdObj = new LessonId(lessonId);
            var query = new GetLessonByIdQuery(courseIdObj, lessonIdObj);

            Result<LessonDetailsDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto.ToApiContract(courseIdObj, linkProvider)),
                CustomResults.Problem);
        })
        .WithMetadata<LessonDetailsResponse>(
            nameof(GetLessonById),
            tag: Tags.Lessons,
            summary: "Gets a lesson by its ID.");
    }
}