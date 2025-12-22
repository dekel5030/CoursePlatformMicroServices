using Application.Courses.Queries.Dtos;
using Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;

namespace Application.Lessons.Queries.GetById;

public record GetLessonByIdQuery(LessonId Id) : IQuery<LessonReadDto>;
