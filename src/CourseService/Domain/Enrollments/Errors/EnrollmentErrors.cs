using Kernel;

namespace Courses.Domain.Enrollments.Errors;

public static class EnrollmentErrors
{
    public static readonly Error UnPublishedCourse = Error.Validation(
        "Enrollment.CourseUnpublished",
        "Cannot enroll in unpublished course");
}