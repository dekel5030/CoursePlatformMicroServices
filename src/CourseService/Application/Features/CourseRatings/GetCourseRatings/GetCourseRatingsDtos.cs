using Courses.Application.Services.LinkProvider.Abstractions.Links;
using Courses.Application.Users.Dtos;

namespace Courses.Application.Features.CourseRatings.GetCourseRatings;

public sealed record GetCourseRatingsDto(
    IReadOnlyList<CourseRatingItemDto> Items,
    int PageNumber,
    int PageSize,
    int TotalItems,
    GetCourseRatingsCollectionLinks Links);

public sealed record CourseRatingItemDto(
    CourseRatingItemData Data,
    CourseRatingItemLinks Links);

public sealed record CourseRatingItemData(
    Guid Id,
    Guid CourseId,
    UserDto User,
    int Rating,
    string Comment,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

public sealed record CourseRatingItemLinks(
    LinkRecord? Update,
    LinkRecord? Delete
);

public sealed record GetCourseRatingsCollectionLinks(
    LinkRecord Self,
    LinkRecord? Next,
    LinkRecord? Prev,
    LinkRecord? Create
);
