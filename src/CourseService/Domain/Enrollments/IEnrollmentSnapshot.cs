using Courses.Domain.Courses.Primitives;
using Courses.Domain.Enrollments.Primitives;
using Courses.Domain.Lessons.Primitives;

namespace Courses.Domain.Enrollments;

public interface IEnrollmentSnapshot
{
    EnrollmentId Id { get; }
    CourseId CourseId { get; }
    UserId StudentId { get; }
    DateTimeOffset EnrolledAt { get; }
    DateTimeOffset ExpiresAt { get; }
    EnrollmentStatus Status { get; }
    DateTimeOffset? CompletedAt { get; }
    LessonId? LastAccessedLessonId { get; }
    DateTimeOffset? LastAccessedAt { get; }
    IReadOnlySet<LessonId> CompletedLessons { get; }
}
