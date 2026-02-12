using Courses.Application.Lessons.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Queries.GetLessons;

public sealed record LessonFilter(
    CourseId? CourseId = null,
    LessonId? Id = null,
    IEnumerable<Guid>? Ids = null,
    bool IncludeDetails = false);

public sealed record GetLessonsQuery(LessonFilter Filter) : IQuery<IReadOnlyList<LessonDto>>;
