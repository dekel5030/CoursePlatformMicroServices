using Application.Abstractions.Messaging;
using Application.Courses.Queries.Dtos;
using Domain.Lessons.Primitives;

namespace Application.Lessons.Queries.GetById;

public record GetLessonByIdQuery(LessonId Id) : IQuery<LessonReadDto>;
