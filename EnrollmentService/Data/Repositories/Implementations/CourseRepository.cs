using EnrollmentService.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentService.Data.Repositories.Implementations;

public class CourseRepository : ICourseRepository
{
    private readonly EnrollmentDbContext _dbContext;

    public CourseRepository(EnrollmentDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task<bool> CourseExistsAsync(int courseId, CancellationToken ct = default)
    {
        return _dbContext.KnownCourses.AnyAsync(c => c.CourseId == courseId, ct);
    }
}
