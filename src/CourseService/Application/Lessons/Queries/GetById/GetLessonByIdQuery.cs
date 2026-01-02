using Courses.Application.Courses.Queries.Dtos;
using Courses.Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Queries.GetById;

public record GetLessonByIdQuery(LessonId Id) : IQuery<LessonDetailsDto>;
