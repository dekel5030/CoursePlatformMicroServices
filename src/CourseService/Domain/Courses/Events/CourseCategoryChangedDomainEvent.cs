using Courses.Domain.Categories.Primitives;
using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses;

public sealed record CourseCategoryChangedDomainEvent(CourseId Id, CategoryId NewCategoryId) : IDomainEvent;
