using Courses.Application.Abstractions.Data;
using Courses.Application.Enrollments.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Enrollments;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Enrollments.Queries.GetEnrollments;

internal sealed class GetEnrollmentsQueryHandler : IQueryHandler<GetEnrollmentsQuery, EnrollmentCollectionDto>
{
    private readonly IReadDbContext _readDbContext;

    public GetEnrollmentsQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<EnrollmentCollectionDto>> Handle(
        GetEnrollmentsQuery request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Enrollment> query = _readDbContext.Enrollments;

        query = ApplyFilters(query, request);

        int totalCount = await query.CountAsync(cancellationToken);

        List<Enrollment> items = await query
            .OrderByDescending(e => e.EnrolledAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        List<EnrollmentDto> dtos = MapToDtoCollection(items);

        var result = new EnrollmentCollectionDto
        {
            Items = dtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalItems = totalCount,
            Links = null
        };

        return Result.Success(result);
    }

    private static List<EnrollmentDto> MapToDtoCollection(List<Enrollment> items)
    {
        return items.Select(enrollment => new EnrollmentDto(
            enrollment.Id.Value,
            enrollment.CourseId.Value,
            enrollment.StudentId.Value,
            enrollment.EnrolledAt,
            enrollment.ExpiresAt,
            enrollment.Status.ToString(),
            enrollment.CompletedAt)).ToList();
    }

    private static IQueryable<Enrollment> ApplyFilters(IQueryable<Enrollment> baseQuery, GetEnrollmentsQuery request)
    {
        if (request.CourseId.HasValue)
        {
            var courseId = new CourseId(request.CourseId.Value);
            baseQuery = baseQuery.Where(e => e.CourseId == courseId);
        }
        if (request.StudentId.HasValue)
        {
            var studentId = new UserId(request.StudentId.Value);
            baseQuery = baseQuery.Where(e => e.StudentId == studentId);
        }

        return baseQuery;
    }
}
