using Courses.Domain.Enrollments;
using Courses.Domain.Enrollments.Primitives;

namespace Courses.Domain.Abstractions.Repositories;

public interface IEnrollmentRepository : IRepository<Enrollment, EnrollmentId>;
