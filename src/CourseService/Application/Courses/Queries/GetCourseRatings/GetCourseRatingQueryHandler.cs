using Courses.Application.Abstractions.Data;
using Courses.Application.Courses.Dtos;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Ratings;
using Courses.Domain.Users;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetCourseRatings;

public sealed class GetCourseRatingQueryHandler
    : IQueryHandler<GetCourseRatingsQuery, CourseRatingCollection>
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

    public async Task<Result<CourseRatingCollection>> Handle(
        GetCourseRatingsQuery request,
        CancellationToken cancellationToken = default)
    {
        var courseId = new CourseId(request.CourseId);
        var currentUserId = new UserId(_userContext.Id ?? Guid.Empty);

        (List<CourseRating> ratings, int totalCount) = await GetPagedRatingsAsync(
            courseId,
            request,
            cancellationToken);

        bool userHasExistingRating = await _dbContext.CourseRatings
                .AnyAsync(rating => rating.CourseId == courseId && rating.UserId == currentUserId, cancellationToken);

        var links = _linkBuilder.BuildLinks(LinkResourceKey.CourseRatingCollection, new CourseRatingCollectionContext(
                courseId,
                currentUserId,
                userHasExistingRating)).ToList();

        if (totalCount == 0)
        {
            return EmptyResult(request, links);
        }

        Dictionary<UserId, User> userMap = await GetUserMapAsync(ratings, cancellationToken);

        var ratingDtos = ratings.Select(rating =>
        {
            var ratingLinkContext = new CourseRatingLinkContext(rating.Id, rating.UserId, currentUserId);
            IReadOnlyList<LinkDto> links = _linkBuilder
                .BuildLinks(LinkResourceKey.CourseRating, ratingLinkContext);

            return MapToDto(rating, userMap.GetValueOrDefault(rating.UserId), links);
        }).ToList();

        var result = new CourseRatingCollection
        {
            Items = ratingDtos,
            TotalItems = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            Links = links
        };

        return Result.Success(result);
    }

    private static Result<CourseRatingCollection> EmptyResult(GetCourseRatingsQuery request, List<LinkDto> links)
    {
        return Result.Success(new CourseRatingCollection
        {
            Items = [],
            Links = links,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalItems = 0
        });
    }

    private async Task<(List<CourseRating> Items, int TotalCount)> GetPagedRatingsAsync(
        CourseId courseId,
        GetCourseRatingsQuery request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<CourseRating> query = _dbContext.CourseRatings
            .Where(r => r.CourseId == courseId);

        int count = await query.CountAsync(cancellationToken);

        List<CourseRating> items = await query
            .OrderByDescending(r => r.CreatedAtUtc)
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
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, cancellationToken);
    }

    private static CourseRatingDto MapToDto(
        CourseRating rating,
        User? user,
        IReadOnlyList<LinkDto> links)
    {
        var emptyUser = new UserDto
        {
            Id = Guid.Empty,
            FirstName = string.Empty,
            LastName = string.Empty,
            AvatarUrl = null
        };

        return new CourseRatingDto
        {
            Id = rating.Id.Value,
            CourseId = rating.CourseId.Value,
            Rating = rating.Score,
            Comment = rating.Comment ?? string.Empty,
            CreatedAt = rating.CreatedAtUtc,
            UpdatedAt = rating.UpdatedAtUtc,
            User = user is null ? emptyUser with { Id = rating.UserId.Value } : new UserDto
            {
                Id = user.Id.Value,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AvatarUrl = user.AvatarUrl
            },
            Links = [.. links]
        };
    }
}
