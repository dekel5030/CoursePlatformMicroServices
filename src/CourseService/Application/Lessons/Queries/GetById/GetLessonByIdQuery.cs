using Courses.Application.Lessons.Queries.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Queries.GetById;

public record GetLessonByIdQuery(CourseId CourseId, LessonId LessonId) : IQuery<LessonDetailsDto>;
