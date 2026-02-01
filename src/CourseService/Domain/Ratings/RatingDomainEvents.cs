using Courses.Domain.Courses.Primitives;
using Courses.Domain.Ratings.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Ratings;

public sealed record CourseRatingCreatedDomainEvent(
    RatingId RatingId,
    CourseId CourseId,
    int Score) : IDomainEvent;

public sealed record CourseRatingUpdatedDomainEvent(
    RatingId RatingId,
    CourseId CourseId,
    int OldScore,
    int NewScore) : IDomainEvent;
