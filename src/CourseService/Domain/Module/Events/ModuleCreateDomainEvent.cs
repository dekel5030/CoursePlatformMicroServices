using System;
using System.Collections.Generic;
using System.Text;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Module.Events;

public sealed record ModuleCreateDomainEvent(IModuleSnapshot Module) : IDomainEvent;
