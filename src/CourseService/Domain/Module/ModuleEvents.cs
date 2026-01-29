using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Module.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Module;

public record ModuleCreatedDomainEvent(ModuleId Id, CourseId CourseId, Title Title, int Index) : IDomainEvent;
public record ModuleTitleChangedDomainEvent(ModuleId Id, CourseId CourseId, Title NewTitle) : IDomainEvent;
public record ModuleIndexUpdatedDomainEvent(ModuleId Id, CourseId CourseId, int NewIndex) : IDomainEvent;
public record ModuleDeletedDomainEvent(ModuleId Id, CourseId CourseId) : IDomainEvent;

