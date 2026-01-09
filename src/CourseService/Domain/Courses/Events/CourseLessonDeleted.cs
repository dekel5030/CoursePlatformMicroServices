using Courses.Domain.Lessons;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses;

public record CourseLessonDeleted(Course Course, Lesson Lesson) : IDomainEvent;