using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses.Events;

public sealed record CourseTagsUpdatedDomainEvent(CourseId Id, IEnumerable<Tag> NewTags) : IDomainEvent;
