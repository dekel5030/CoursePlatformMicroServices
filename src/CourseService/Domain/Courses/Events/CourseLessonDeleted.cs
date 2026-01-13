using Courses.Domain.Lessons;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses.Events;

public record CourseLessonDeleted(Course Course, Lesson Lesson) : IDomainEvent;