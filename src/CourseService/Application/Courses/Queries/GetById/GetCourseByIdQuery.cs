using Courses.Application.Courses.Queries.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetById;

public record GetCourseByIdQuery(CourseId Id) : IQuery<CourseDetailsDto>;