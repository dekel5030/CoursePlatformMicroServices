using Courses.Domain.Enrollments.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Enrollments.Commands.MarkLessonAsCompleted;

public sealed record MarkLessonAsCompletedCommand(
    EnrollmentId EnrollmentId,
    LessonId LessonId) : ICommand;
