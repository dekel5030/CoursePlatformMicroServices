using Application.Abstractions.Messaging;
using Application.Courses.Queries.Dtos;
using Application.Lessons.Queries.GetById;
using Domain.Lessons.Primitives;
using Kernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Lessons;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/lessons/{id:Guid}", async (
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
