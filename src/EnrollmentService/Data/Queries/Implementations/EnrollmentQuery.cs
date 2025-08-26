using EnrollmentService.Data.Queries.Interfaces;
using EnrollmentService.Dtos;
using EnrollmentService.Models;
using EnrollmentService.Options;

namespace EnrollmentService.Data.Queries.Implementations;

public sealed class EnrollmentQuery : IQueryObject<Enrollment>, IPageableQuery<Enrollment>, ISortableQuery<Enrollment>
{
    public int? Id { get; private set; }
    public int? CourseId { get; private set; }
    public int? UserId { get; private set; }
    public EnrollmentStatus? Status { get; private set; }
    public DateTime? EnrolledAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    public int PageNumber { get; private set; }
    public int PageSize { get; private set; }

    private EnrollmentQuery() {}

    public static EnrollmentQuery FromDto(EnrollmentSearchDto dto, PaginationOptions pagingOptions) =>
        new()
        {
            Id = dto.Id,
            CourseId = dto.CourseId,
            UserId = dto.UserId,
            Status = dto.Status,
            EnrolledAt = dto.EnrolledAt,
            UpdatedAt = dto.UpdatedAt,
            ExpiresAt = dto.ExpiresAt,
            PageNumber = Math.Max(dto.PageNumber ?? 1, 1),
            PageSize = Math.Clamp(dto.PageSize ?? pagingOptions.DefaultPageSize, 1, pagingOptions.MaxPageSize)
        };

    public IQueryable<Enrollment> Apply(IQueryable<Enrollment> q)
    {
        if (q is null) throw new ArgumentNullException(nameof(q));

        q = FilterById(q);
        q = FilterByCourseId(q);
        q = FilterByUserId(q);
        q = FilterByStatus(q);
        q = FilterByEnrolledAt(q);
        q = FilterByUpdatedAt(q);
        q = FilterByExpiresAt(q);
        q = ApplyDefaultSorting(q);

        return q;
    }

    public IQueryable<Enrollment> ApplyPagination(IOrderedQueryable<Enrollment> query)
    {
        return query
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize);
    }

    public IOrderedQueryable<Enrollment> ApplySorting(IQueryable<Enrollment> query)
    {
        return query.OrderByDescending(e => e.EnrolledAt);
    }

    private IQueryable<Enrollment> FilterById(IQueryable<Enrollment> query)
    {
        if (Id == null)
        {
            return query;
        }

        return query.Where(e => e.Id == Id);
    }

    private IQueryable<Enrollment> FilterByCourseId(IQueryable<Enrollment> query)
    {
        if (CourseId == null)
        {
            return query;
        }

        return query.Where(e => e.CourseId == CourseId);
    }

    private IQueryable<Enrollment> FilterByUserId(IQueryable<Enrollment> query)
    {
        if (UserId == null)
        {
            return query;
        }

        return query.Where(e => e.UserId == UserId);
    }

    private IQueryable<Enrollment> FilterByStatus(IQueryable<Enrollment> query)
    {
        if (Status == null)
        {
            return query;
        }

        return query.Where(e => e.Status == Status);
    }

    private IQueryable<Enrollment> FilterByEnrolledAt(IQueryable<Enrollment> query)
    {
        if (EnrolledAt == null)
        {
            return query;
        }

        return query.Where(e => e.EnrolledAt == EnrolledAt);
    }

    private IQueryable<Enrollment> FilterByUpdatedAt(IQueryable<Enrollment> query)
    {
        if (UpdatedAt == null)
        {
            return query;
        }

        return query.Where(e => e.UpdatedAt == UpdatedAt);
    }

    private IQueryable<Enrollment> FilterByExpiresAt(IQueryable<Enrollment> query)
    {
        if (ExpiresAt == null)
        {
            return query;
        }

        return query.Where(e => e.ExpiresAt == ExpiresAt);
    }

    private static IQueryable<Enrollment> ApplyDefaultSorting(IQueryable<Enrollment> q)
    {
        return q.OrderByDescending(e => e.EnrolledAt);
    }
}