using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Enrollments.Queries.Dtos;
using Domain.Users.Primitives;
using Domain.Courses.Primitives;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Enrollments.Queries.GetEnrollments;

public sealed class GetEnrollmentsQueryHandler
    : IQueryHandler<GetEnrollmentsQuery, PagedResponse<EnrollmentReadDto>>
{
    private readonly IReadDbContext _dbContext;

    public GetEnrollmentsQueryHandler(IReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResponse<EnrollmentReadDto>>> Handle(
        GetEnrollmentsQuery query,
        CancellationToken cancellationToken = default)
    {
        var queryable = _dbContext.Enrollments.AsQueryable();

        if (query.UserId.HasValue)
        {
            var userId = new ExternalUserId(query.UserId.Value);
            queryable = queryable.Where(e => e.UserId == userId);
        }

        if (query.CourseId.HasValue)
        {
            var courseId = new ExternalCourseId(query.CourseId.Value);
            queryable = queryable.Where(e => e.CourseId == courseId);
        }

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(e => e.Status == query.Status.Value);
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        var enrollments = await queryable
            .OrderByDescending(e => e.EnrolledAt)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(e => new EnrollmentReadDto
            {
                Id = e.Id.Value,
                UserId = e.UserId.Value,
                CourseId = e.CourseId.Value,
                Status = e.Status,
                EnrolledAt = e.EnrolledAt,
                ExpiresAt = e.ExpiresAt
            })
            .ToListAsync(cancellationToken);

        return Result.Success(new PagedResponse<EnrollmentReadDto>
        {
            Items = enrollments,
            TotalCount = totalCount,
            PageSize = query.PageSize,
            PageNumber = query.PageNumber
        });
    }
}
