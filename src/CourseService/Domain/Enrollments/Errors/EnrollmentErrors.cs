using Kernel;

namespace Courses.Domain.Enrollments.Errors;

public static class EnrollmentErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Enrollment.NotFound",
        "The specified enrollment was not found.");

    public static readonly Error UnPublishedCourse = Error.Validation(
        "Enrollment.CourseUnpublished",
        "Cannot enroll in unpublished course");

    public static readonly Error InvalidExpirationDate = Error.Validation(
        "Enrollment.InvalidExpirationDate",
        "Expiration date must be after enrollment date");

    public static readonly Error Unauthenticated = Error.Unauthorized(
        "Enrollment.Unauthenticated",
        "User must be authenticated to list enrolled courses.");
}
