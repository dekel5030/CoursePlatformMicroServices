using EnrollmentService.Data.Queries.Interfaces;
using EnrollmentService.Models;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentService.Data;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly EnrollmentDbContext _dbContext;

    public EnrollmentRepository(EnrollmentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Enrollment?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return _dbContext.Enrollments
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<(IReadOnlyList<Enrollment> enrollments, int totalCount)> SearchEnrollmentsAsync(
        IQueryObject<Enrollment> query, CancellationToken ct = default)
    {
        IQueryable<Enrollment> baseQuery = _dbContext.Enrollments.AsNoTracking();

        IQueryable<Enrollment> filtered = query.Apply(baseQuery);

        int totalCount = await filtered.CountAsync(ct);

        IOrderedQueryable<Enrollment> ordered =
            (query is ISortableQuery<Enrollment> sortable)
                ? sortable.ApplySorting(filtered)
                : filtered.OrderBy(e => e.Id);

        IQueryable<Enrollment> paged =
            (query is IPageableQuery<Enrollment> pageable)
                ? pageable.ApplyPagination(ordered)
                : ordered;

        var items = await paged.ToListAsync(ct);

        return (items, totalCount);
    }

    public Task AddAsync(Enrollment enrollment, CancellationToken ct = default)
    {
        _dbContext.Enrollments.AddAsync(enrollment, ct);
        return Task.CompletedTask;
    }

    public void Remove(Enrollment enrollment)
    {
        _dbContext.Enrollments.Remove(enrollment);
    }

    public Task<bool> ExistsAsync(int courseId, int userId, CancellationToken ct = default)
    {
        return _dbContext.Enrollments
            .AsNoTracking()
            .AnyAsync(e => e.CourseId == courseId && e.UserId == userId, ct);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return _dbContext.SaveChangesAsync(ct);
    }
}
