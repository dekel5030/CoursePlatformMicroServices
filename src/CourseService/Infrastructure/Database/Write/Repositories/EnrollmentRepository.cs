using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Enrollments;
using Courses.Domain.Enrollments.Primitives;
using Courses.Infrastructure.Database.Write;

namespace Courses.Infrastructure.Database.Write.Repositories;

public class EnrollmentRepository : RepositoryBase<Enrollment, EnrollmentId>, IEnrollmentRepository
{
    public EnrollmentRepository(WriteDbContext dbContext) : base(dbContext)
    {
    }
}
