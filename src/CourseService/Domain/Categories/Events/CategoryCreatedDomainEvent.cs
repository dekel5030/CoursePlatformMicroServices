using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Categories.Events;

public sealed record CategoryCreatedDomainEvent(ICategorySnapshot Category) : IDomainEvent;
