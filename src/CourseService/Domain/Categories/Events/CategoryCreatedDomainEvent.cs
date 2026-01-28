using System;
using System.Collections.Generic;
using System.Text;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Categories.Events;
public sealed record CategoryCreatedDomainEvent(ICategorySnapshot Category) : IDomainEvent;
