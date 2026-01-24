using System.Text.Json.Serialization;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Courses.Queries.GetById;

public record LessonDto(
    Guid LessonId,
    string Title,
    int Index,
    TimeSpan Duration,
    string? ThumbnailUrl,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] LessonAccess Access,
    IReadOnlyList<LinkDto> Links
);
