using EnrollmentService.Data.Queries.Interfaces;
using EnrollmentService.Dtos;
using EnrollmentService.Models;

namespace EnrollmentService.Data.Queries.Implementations;

public sealed class EnrollmentQuery : IQueryObject<Enrollment>, IPageableQuery<Enrollment>, ISortableQuery<Enrollment>
{
    private readonly EnrollmentSearchDto _dto;

    public int PageNumber => _dto.PageNumber;
    public int PageSize => _dto.PageSize;

    public EnrollmentQuery(EnrollmentSearchDto dto)
    {
        _dto = dto ?? throw new ArgumentNullException(nameof(dto));
    }

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
        throw new NotImplementedException();
    }

    private IQueryable<Enrollment> FilterById(IQueryable<Enrollment> query)
    {
        if (_dto.Id == null)
        {
            return query;
        }

        return query.Where(e => e.Id == _dto.Id);
    }

    private IQueryable<Enrollment> FilterByCourseId(IQueryable<Enrollment> query)
    {
        if (_dto.CourseId == null)
        {
            return query;
        }

        return query.Where(e => e.CourseId == _dto.CourseId);
    }

    private IQueryable<Enrollment> FilterByUserId(IQueryable<Enrollment> query)
    {
        if (_dto.UserId == null)
        {
            return query;
        }

        return query.Where(e => e.UserId == _dto.UserId);
    }

    private IQueryable<Enrollment> FilterByStatus(IQueryable<Enrollment> query)
    {
        if (_dto.Status == null)
        {
            return query;
        }

        return query.Where(e => e.Status == _dto.Status);
    }

    private IQueryable<Enrollment> FilterByEnrolledAt(IQueryable<Enrollment> query)
    {
        if (_dto.EnrolledAt == null)
        {
            return query;
        }

        return query.Where(e => e.EnrolledAt == _dto.EnrolledAt);
    }

    private IQueryable<Enrollment> FilterByUpdatedAt(IQueryable<Enrollment> query)
    {
        if (_dto.UpdatedAt == null)
        {
            return query;
        }

        return query.Where(e => e.UpdatedAt == _dto.UpdatedAt);
    }

    private IQueryable<Enrollment> FilterByExpiresAt(IQueryable<Enrollment> query)
    {
        if (_dto.ExpiresAt == null)
        {
            return query;
        }

        return query.Where(e => e.ExpiresAt == _dto.ExpiresAt);
    }

    private static IQueryable<Enrollment> ApplyDefaultSorting(IQueryable<Enrollment> q)
    {
        return q.OrderByDescending(e => e.EnrolledAt);
    }

}