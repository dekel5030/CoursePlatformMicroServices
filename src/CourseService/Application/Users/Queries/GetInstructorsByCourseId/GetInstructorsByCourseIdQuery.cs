using Courses.Application.Abstractions.Data;
using Courses.Application.Courses.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Users.Queries.GetInstructorsByCourseId;

public sealed record GetInstructorsByCourseIdQuery(CourseId CourseId) : IQuery<IReadOnlyList<UserDto>>;
