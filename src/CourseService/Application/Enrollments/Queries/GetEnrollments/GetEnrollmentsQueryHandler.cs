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

        if (request.CourseId.HasValue)
        {
            var courseId = new CourseId(request.CourseId.Value);
            query = query.Where(e => e.CourseId == courseId);
        }

        if (request.StudentId.HasValue)
        {
            var studentId = new UserId(request.StudentId.Value);
            query = query.Where(e => e.StudentId == studentId);
        }

        int totalCount = await query.CountAsync(cancellationToken);

        List<Enrollment> items = await query
            .OrderByDescending(e => e.EnrolledAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = items.Select(e => new EnrollmentDto(
            e.Id.Value,
            e.CourseId.Value,
            e.StudentId.Value,
            e.EnrolledAt,
            e.ExpiresAt,
            e.Status.ToString(),
            e.CompletedAt)).ToList();

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
}
