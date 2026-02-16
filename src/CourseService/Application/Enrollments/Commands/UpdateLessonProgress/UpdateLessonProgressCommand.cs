using Courses.Domain.Enrollments.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Enrollments.Commands.UpdateLessonProgress;

public sealed record UpdateLessonProgressCommand(
    EnrollmentId EnrollmentId,
    LessonId LessonId,
    int Seconds) : ICommand;
