using System;
using System.Collections.Generic;
using System.Text;

namespace CoursePlatform.Contracts.CourseEvents;

public sealed record CourseCreatedIntegrationEvent(
    Guid CourseId,
    Guid InstructorId,
    string Title,
    string Description,
    decimal PriceAmount,
    string PriceCurrency,
    string Status,
    string Slug,
    string Difficulty,
    string Language,
    Guid CategoryId);

public sealed record CourseCategoryChangedIntegrationEvent(Guid CourseId, Guid NewCategoryId);

public sealed record CourseDescriptionChangedIntegrationEvent(Guid CourseId, string NewDescription);

public sealed record CourseDifficultyChangedIntegrationEvent(Guid CourseId, string NewDifficulty);

public sealed record CourseImageAddedIntegrationEvent(Guid CourseId, string ImageUrl);

public sealed record CourseImageRemovedIntegrationEvent(Guid CourseId, string ImageUrl);

public sealed record CourseLanguageChangedIntegrationEvent(Guid CourseId, string NewLanguage);

public sealed record CoursePriceChangedIntegrationEvent(Guid CourseId, decimal NewAmount, string NewCurrency);

public sealed record CourseSlugChangedIntegrationEvent(Guid CourseId, string NewSlug);

public sealed record CourseStatusChangedIntegrationEvent(Guid CourseId, string NewStatus);

public sealed record CourseTagsUpdatedIntegrationEvent(Guid CourseId, List<string> NewTags);

public sealed record CourseTitleChangedIntegrationEvent(Guid CourseId, string NewTitle);