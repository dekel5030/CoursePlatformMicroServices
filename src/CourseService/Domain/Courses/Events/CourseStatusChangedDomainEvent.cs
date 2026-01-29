using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses.Events;

public sealed record CourseStatusChangedDomainEvent(CourseId Id, CourseStatus NewStatus) : IDomainEvent;
