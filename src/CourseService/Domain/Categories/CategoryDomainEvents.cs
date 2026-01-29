using Courses.Domain.Categories.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Categories;

public record CategoryCreatedDomainEvent(CategoryId Id, string Name, Slug Slug) : IDomainEvent;

public record CategoryRenamedDomainEvent(CategoryId Id, string NewName, Slug NewSlug) : IDomainEvent;