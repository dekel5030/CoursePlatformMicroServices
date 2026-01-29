using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses.Events;

public sealed record CourseLanguageChangedDomainEvent(CourseId Id, Language NewLanguage) : IDomainEvent;
