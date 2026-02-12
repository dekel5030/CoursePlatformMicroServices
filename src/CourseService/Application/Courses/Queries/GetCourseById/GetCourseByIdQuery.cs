using Courses.Application.Courses.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetCourseById;

public record GetCourseByIdQuery(CourseId Id) : IQuery<CourseDto>;
