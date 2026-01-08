using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Lessons.Events;

public record LessonVideoDataUpdatedDomainEvent(Lesson Lesson) : IDomainEvent;