using Courses.Domain.Categories.Primitives;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses.Events;

public sealed record CourseCreatedDomainEvent(
    CourseId Id,
    UserId InstructorId,
    Title Title,
    Description Description,
    Money Price,
    CourseStatus Status,
    Slug Slug,
    DifficultyLevel Difficulty,
    Language Language,
    CategoryId CategoryId) : IDomainEvent;
