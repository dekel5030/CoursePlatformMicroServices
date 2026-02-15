using Courses.Application.Abstractions.Data;
using Courses.Application.Features.CourseRatings.GetCourseRatings;
using Courses.Application.Services.Actions;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Services.LinkProvider.Abstractions.Links;
using Courses.Application.Users;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Ratings;
using Courses.Domain.Ratings.Primitives;
using Courses.Domain.Users;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Features.CourseRatings.GetCourseRatings;

public sealed class GetCourseRatingQueryHandler
    : IQueryHandler<GetCourseRatingsQuery, GetCourseRatingsDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly ILinkProvider _linkProvider;
    private readonly IUserContext _userContext;

    public GetCourseRatingQueryHandler(
        IReadDbContext dbContext,
        ILinkProvider linkProvider,
        IUserContext userContext)
    {
        _dbContext = dbContext;
        _linkProvider = linkProvider;
        _userContext = userContext;
    }

    public async Task<Result<GetCourseRatingsDto>> Handle(
        GetCourseRatingsQuery request,
        CancellationToken cancellationToken = default)
    {
        var courseId = new CourseId(request.CourseId);
        UserId? currentUserId = _userContext.Id is not null ? new UserId(_userContext.Id.Value) : null;

        (List<CourseRating> ratings, int totalCount) =
            await GetPagedRatingsAsync(courseId, request, cancellationToken);

        bool hasRated = await CheckIfUserRatedAsync(courseId, currentUserId, cancellationToken);

        GetCourseRatingsCollectionLinks collectionLinks = BuildCollectionLinks(
            request.CourseId, currentUserId, hasRated);

        if (totalCount == 0)
        {
            return Result.Success(new GetCourseRatingsDto(
                Items: [],
                PageNumber: request.PageNumber,
                PageSize: request.PageSize,
                TotalItems: 0,
                Links: collectionLinks));
        }

        Dictionary<UserId, User> userMap = await GetUserMapAsync(ratings, cancellationToken);
        List<CourseRatingItemDto> items = MapToItemDtos(ratings, userMap, currentUserId);

        return Result.Success(new GetCourseRatingsDto(
            Items: items,
            PageNumber: request.PageNumber,
            PageSize: request.PageSize,
            TotalItems: totalCount,
            Links: collectionLinks));
    }

    private async Task<bool> CheckIfUserRatedAsync(
        CourseId courseId,
        UserId? userId,
        CancellationToken cancellationToken = default)
    {
        if (userId is null)
        {
            return false;
        }

        return await _dbContext.CourseRatings
            .AnyAsync(rating =>
                rating.CourseId == courseId &&
                rating.UserId == userId,
                cancellationToken);
    }

    private GetCourseRatingsCollectionLinks BuildCollectionLinks(
        Guid courseId,
        UserId? currentUserId,
        bool hasRated)
    {
        var context = new CourseRatingCollectionContext(
            new CourseId(courseId),
            currentUserId,
            hasRated);
        bool canCreate = CourseRatingGovernancePolicy.CanCreateRating(context);

        return new GetCourseRatingsCollectionLinks(
            Self: _linkProvider.GetCourseRatingsLink(courseId),
            CreateRating: canCreate ? _linkProvider.GetCreateCourseRatingLink(courseId) : null);
    }

    private List<CourseRatingItemDto> MapToItemDtos(
        List<CourseRating> ratings,
        Dictionary<UserId, User> userMap,
        UserId? currentUserId)
    {
        return ratings.Select(rating =>
        {
            var linkContext = new CourseRatingLinkContext(
                rating.Id,
                rating.UserId,
                currentUserId);
            bool canUpdate = CourseRatingGovernancePolicy.CanUpdateRating(linkContext);
            bool canDelete = CourseRatingGovernancePolicy.CanDeleteRating(linkContext);

            var links = new CourseRatingItemLinks(
                Update: canUpdate ? _linkProvider.GetUpdateCourseRatingLink(rating.Id.Value) : null,
                Delete: canDelete ? _linkProvider.GetDeleteCourseRatingLink(rating.Id.Value) : null);

            var data = new CourseRatingItemData(
                Id: rating.Id.Value,
                CourseId: rating.CourseId.Value,
                User: UserDtoMapper.Map(userMap.GetValueOrDefault(rating.UserId), rating.UserId.Value),
                Rating: rating.Score,
                Comment: rating.Comment ?? string.Empty,
                CreatedAt: rating.CreatedAtUtc,
                UpdatedAt: rating.UpdatedAtUtc);

            return new CourseRatingItemDto(Data: data, Links: links);
        }).ToList();
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
}
