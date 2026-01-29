using Courses.Domain.Categories.Primitives;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses;

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


public sealed record CourseDomainEvents(CourseId Id, CategoryId NewCategoryId) : IDomainEvent;

public sealed record CourseDescriptionChangedDomainEvent(CourseId Id, Description NewDescription) : IDomainEvent;

public sealed record CourseDifficultyChangedDomainEvent(CourseId Id, DifficultyLevel NewDifficulty) : IDomainEvent;

public sealed record CourseImageAddedDomainEvent(CourseId Id, ImageUrl ImageUrl) : IDomainEvent;

public sealed record CourseImageRemovedDomainEvent(CourseId Id, ImageUrl ImageUrl) : IDomainEvent;

public sealed record CourseLanguageChangedDomainEvent(CourseId Id, Language NewLanguage) : IDomainEvent;

public sealed record CoursePriceChangedDomainEvent(CourseId Id, Money NewPrice) : IDomainEvent;

public sealed record CourseSlugChangedDomainEvent(CourseId Id, Slug NewSlug) : IDomainEvent;

public sealed record CourseStatusChangedDomainEvent(CourseId Id, CourseStatus NewStatus) : IDomainEvent;

public sealed record CourseTagsUpdatedDomainEvent(CourseId Id, IEnumerable<Tag> NewTags) : IDomainEvent;

public sealed record CourseTitleChangedDomainEvent(CourseId Id, Title NewTitle) : IDomainEvent;
