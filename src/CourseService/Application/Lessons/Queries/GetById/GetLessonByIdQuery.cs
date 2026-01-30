using Courses.Application.Lessons.Dtos;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Queries.GetById;

public record GetLessonByIdQuery(ModuleId ModuleId, LessonId LessonId) : IQuery<LessonDetailsPageDto>;
