using Courses.Domain.Courses.Primitives;
using Courses.Domain.Modules.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Modules;

public record ModuleCreatedDomainEvent(ModuleId Id, CourseId CourseId, Title Title, int Index) : IDomainEvent;
public record ModuleTitleChangedDomainEvent(ModuleId Id, CourseId CourseId, Title NewTitle) : IDomainEvent;
public record ModuleIndexUpdatedDomainEvent(ModuleId Id, CourseId CourseId, int NewIndex) : IDomainEvent;
public record ModuleDeletedDomainEvent(ModuleId Id, CourseId CourseId) : IDomainEvent;

