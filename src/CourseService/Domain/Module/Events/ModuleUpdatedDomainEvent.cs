using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Module.Events;

public sealed record ModuleUpdatedDomainEvent(IModuleSnapshot Module) : IDomainEvent;
