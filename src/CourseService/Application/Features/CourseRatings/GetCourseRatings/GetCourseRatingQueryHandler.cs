using Courses.Application.Abstractions.Data;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Users;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Ratings;
using Courses.Domain.Users;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Features.CourseRatings.GetCourseRatings;

public sealed class GetCourseRatingQueryHandler
    : IQueryHandler<GetCourseRatingsQuery, CourseRatingCollectionDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly ILinkBuilderService _linkBuilder;
    private readonly IUserContext _userContext;

    public GetCourseRatingQueryHandler(
        IReadDbContext dbContext,
        ILinkBuilderService linkBuilder,
        IUserContext userContext)
    {
        _dbContext = dbContext;
        _linkBuilder = linkBuilder;
        _userContext = userContext;
    }

    public async Task<Result<CourseRatingCollectionDto>> Handle(
        GetCourseRatingsQuery request,
        CancellationToken cancellationToken = default)
    {
        var courseId = new CourseId(request.CourseId);
        var currentUserId = new UserId(_userContext.Id ?? Guid.Empty);

        (List<CourseRating>? ratings, int totalCount) =
            await GetPagedRatingsAsync(courseId, request, cancellationToken);

        bool hasRated = await CheckIfUserRatedAsync(courseId, currentUserId, cancellationToken);

        List<LinkDto> collectionLinks = BuildCollectionLinks(courseId, currentUserId, hasRated);

        if (totalCount == 0)
        {
            return Result.Success(CreateEmptyResponse(request, collectionLinks));
        }

        Dictionary<UserId, User> userMap = await GetUserMapAsync(ratings, cancellationToken);
        List<CourseRatingDto> items = MapToRatingDtos(ratings, userMap, currentUserId);

        return Result.Success(new CourseRatingCollectionDto
        {
            Items = items,
            TotalItems = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            Links = collectionLinks
        });
    }

    private async Task<bool> CheckIfUserRatedAsync(
        CourseId courseId,
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.CourseRatings
            .AnyAsync(rating =>
                rating.CourseId == courseId &&
                rating.UserId == userId,
                cancellationToken);
    }

    private List<LinkDto> BuildCollectionLinks(CourseId courseId, UserId userId, bool hasRated)
    {
        var context = new CourseRatingCollectionContext(courseId, userId, hasRated);
        return _linkBuilder.BuildLinks(LinkResourceKey.CourseRatingCollection, context).ToList();
    }

    private List<CourseRatingDto> MapToRatingDtos(
        List<CourseRating> ratings,
        Dictionary<UserId, User> userMap,
        UserId currentUserId)
    {
        return ratings.Select(rating =>
        {
            var linkContext = new CourseRatingLinkContext(rating.Id, rating.UserId, currentUserId);
            IReadOnlyList<LinkDto> links = _linkBuilder.BuildLinks(LinkResourceKey.CourseRating, linkContext);

            return MapToDto(rating, userMap.GetValueOrDefault(rating.UserId), links);
        }).ToList();
    }

    private static CourseRatingCollectionDto CreateEmptyResponse(
        GetCourseRatingsQuery request,
        List<LinkDto> links)
    {
        return new CourseRatingCollectionDto
        {
            Items = [],
            TotalItems = 0,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            Links = links
        };
    }

    private async Task<(List<CourseRating> Items, int TotalCount)> GetPagedRatingsAsync(
        CourseId courseId,
        GetCourseRatingsQuery request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<CourseRating> query = _dbContext.CourseRatings
            .Where(rating => rating.CourseId == courseId);

        int count = await query.CountAsync(cancellationToken);

        List<CourseRating> items = await query
            .OrderByDescending(rating => rating.CreatedAtUtc)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, count);
    }

    private async Task<Dictionary<UserId, User>> GetUserMapAsync(
        List<CourseRating> ratings,
        CancellationToken cancellationToken = default)
    {
        var userIds = ratings.Select(r => r.UserId).Distinct().ToList();

        return await _dbContext.Users
            .Where(user => userIds.Contains(user.Id))
            .ToDictionaryAsync(user => user.Id, cancellationToken);
    }

    private static CourseRatingDto MapToDto(
        CourseRating rating,
        User? user,
        IReadOnlyList<LinkDto> links)
    {
        return new CourseRatingDto
        {
            Id = rating.Id.Value,
            CourseId = rating.CourseId.Value,
            Rating = rating.Score,
            Comment = rating.Comment ?? string.Empty,
            CreatedAt = rating.CreatedAtUtc,
            UpdatedAt = rating.UpdatedAtUtc,
            User = UserDtoMapper.Map(user, rating.UserId.Value),
            Links = [.. links]
        };
    }
}
