using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Lessons.Queries.GetById;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Module.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Lessons;

internal sealed class GetLessonById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("modules/{moduleId:Guid}/lessons/{lessonId:Guid}", async (
            Guid moduleId,
            Guid lessonId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var moduleIdObj = new ModuleId(moduleId);
            var lessonIdObj = new LessonId(lessonId);
            var query = new GetLessonByIdQuery(moduleIdObj, lessonIdObj);

            Result<LessonDetailsPageDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto),
                CustomResults.Problem);
        })
        .WithMetadata<LessonDetailsPageDto>(
            nameof(GetLessonById),
            tag: Tags.Lessons,
            summary: "Gets a lesson by its ID.");
    }
}
