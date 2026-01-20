using Courses.Application.Abstractions.Repositories;
using Courses.Domain.Enrollments;
using Courses.Domain.Enrollments.Primitives;

namespace Courses.Infrastructure.Database.Repositories;

public class EnrollmentRepository : RepositoryBase<Enrollment, EnrollmentId>, IEnrollmentRepository
{
    public EnrollmentRepository(WriteDbContext dbContext) : base(dbContext)
    {
    }
}
