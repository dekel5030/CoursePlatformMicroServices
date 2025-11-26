using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Enrollments.Queries.Dtos;
using Domain.Courses.Primitives;
using Domain.Users.Primitives;
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
        var queryParams = query.Params;
        var queryable = _dbContext.Enrollments.AsQueryable();

        if (queryParams.UserId is not null)
        {
            var userId = new ExternalUserId(queryParams.UserId);
            queryable = queryable.Where(e => e.UserId == userId);
        }

        if (queryParams.CourseId is not null)
        {
            var courseId = new ExternalCourseId(queryParams.CourseId);
            queryable = queryable.Where(e => e.CourseId == courseId);
        }

        if (queryParams.Status is not null)
        {
            queryable = queryable.Where(e => e.Status.ToString() == queryParams.Status);
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        var enrollments = await queryable
            .OrderByDescending(e => e.EnrolledAt)
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .Select(e => new EnrollmentReadDto
            {
                Id = e.Id.Value.ToString(),
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
            PageSize = queryParams.PageSize,
            PageNumber = queryParams.PageNumber
        });
    }
}