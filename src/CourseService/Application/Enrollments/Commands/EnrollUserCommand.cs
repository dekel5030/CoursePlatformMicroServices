using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Enrollments.Commands;

public sealed record EnrollUserCommand(UserId UserId, CourseId CourseId, TimeSpan ValidFor) : ICommand;
