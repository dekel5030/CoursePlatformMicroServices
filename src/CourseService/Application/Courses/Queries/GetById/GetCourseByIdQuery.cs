using Application.Courses.Queries.Dtos;
using Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Application.Courses.Queries.GetById;

public record GetCourseByIdQuery(CourseId Id) : IQuery<CourseReadDto>;