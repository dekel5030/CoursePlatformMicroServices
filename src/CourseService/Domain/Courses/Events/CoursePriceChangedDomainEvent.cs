using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses.Events;

public sealed record CoursePriceChangedDomainEvent(CourseId Id, Money NewPrice) : IDomainEvent;
