using Application.Abstractions.Messaging;
using Application.Courses.Queries.Dtos;
using Domain.Courses.Primitives;

namespace Application.Courses.Queries.GetById;

public record GetCourseByIdQuery(CourseId Id) : IQuery<CourseReadDto>;