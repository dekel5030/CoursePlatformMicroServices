using CourseService.Models;

namespace CourseService.Data.EnrollmentsRepo;

public class EnrollmentsRepo : IEnrollmentsRepo
{
    private readonly CourseDbContext _dbContext;

    public EnrollmentsRepo(CourseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Enrollment newEnrollment, CancellationToken ct = default)
    {
        await _dbContext.Enrollments.AddAsync(newEnrollment, ct);
    }

    public async Task<Enrollment?> GetByIdAsync(int enrollmentId, CancellationToken ct = default)
    {
        return await _dbContext.Enrollments.FindAsync(new object[] { enrollmentId }, ct);
    }
}
