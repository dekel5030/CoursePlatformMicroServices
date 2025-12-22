using Courses.Api.Extensions;
using Courses.Api.Infrastructure;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Application.Lessons.Queries.GetById;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Lessons;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("lessons/{id:Guid}", async (
            Guid id,
            IQueryHandler<GetLessonByIdQuery, LessonReadDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetLessonByIdQuery(new LessonId(id));

            Result<LessonReadDto> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithName("GetLessonById")
        .WithTags(Tags.Lessons);
    }
}
