using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses.Events;

public sealed record CourseDescriptionChangedDomainEvent(CourseId Id, Description NewDescription) : IDomainEvent;
