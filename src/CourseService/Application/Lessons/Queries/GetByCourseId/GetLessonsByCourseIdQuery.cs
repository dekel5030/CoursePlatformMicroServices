using Courses.Application.Courses.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Queries.GetByCourseId;

public sealed record GetLessonsByCourseIdQuery(CourseId CourseId) : IQuery<IReadOnlyList<LessonDto>>;
