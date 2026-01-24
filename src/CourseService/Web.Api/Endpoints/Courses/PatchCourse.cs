using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Courses.Commands.PatchCourse;
using Courses.Domain.Categories.Primitives;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Courses;

internal sealed class PatchCourse : IEndpoint
{
    internal sealed record PatchCourseRequest(
        string? Title,
        string? Description,
        decimal? PriceAmount,
        string? PriceCurrency,
        string? Difficulty,
        Guid? CategoryId,
        string? Language,
        IReadOnlyList<string>? Tags,
        string? Slug);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("courses/{id:Guid}", async (
            Guid id,
            PatchCourseRequest request,
            IMediator mediator) =>
        {
            Title? title = string.IsNullOrWhiteSpace(request.Title) ? null : new Title(request.Title);
            Description? description = string.IsNullOrWhiteSpace(request.Description) ? null : new Description(request.Description);
            Money? price = request.PriceAmount.HasValue && !string.IsNullOrWhiteSpace(request.PriceCurrency)
                ? new Money(request.PriceAmount.Value, request.PriceCurrency)
                : null;
            DifficultyLevel? difficulty = !string.IsNullOrWhiteSpace(request.Difficulty) && Enum.TryParse<DifficultyLevel>(request.Difficulty, out DifficultyLevel diff)
                ? diff
                : null;
            CategoryId? categoryId = request.CategoryId.HasValue
                ? new CategoryId(request.CategoryId.Value)
                : null;
            Language? language = !string.IsNullOrWhiteSpace(request.Language)
                ? Language.Parse(request.Language)
                : null;

            var command = new PatchCourseCommand(
                new CourseId(id),
                title,
                description,
                price,
                difficulty,
                categoryId,
                language,
                request.Tags,
                request.Slug);

            Result result = await mediator.Send(command);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
        .WithMetadata<EmptyResult>(nameof(PatchCourse), Tags.Courses, "Patch Course");
    }
}
