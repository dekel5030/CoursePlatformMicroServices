using Kernel;

namespace Domain.Enrollments.Errors;

public static class EnrollmentErrors
{
    public static readonly Error EnrollmentNotFound = new(
        "Enrollment.NotFound",
        "The enrollment with the specified identifier was not found",
        ErrorType.NotFound);

    public static readonly Error EnrollmentAlreadyExists = new(
        "Enrollment.AlreadyExists",
        "An enrollment for this user and course already exists",
        ErrorType.Conflict);

    public static readonly Error CannotEnrollInactiveCourse = new(
        "Enrollment.CannotEnrollInactiveCourse",
        "Cannot enroll in an inactive course",
        ErrorType.Validation);

    public static readonly Error CannotEnrollInactiveUser = new(
        "Enrollment.CannotEnrollInactiveUser",
        "Cannot enroll an inactive user",
        ErrorType.Validation);

    public static readonly Error EnrollmentExpired = new(
        "Enrollment.Expired",
        "The enrollment has expired",
        ErrorType.Validation);

    public static readonly Error EnrollmentAlreadyCancelled = new(
        "Enrollment.AlreadyCancelled",
        "The enrollment is already cancelled",
        ErrorType.Validation);

    public static readonly Error EnrollmentAlreadyCompleted = new(
        "Enrollment.AlreadyCompleted",
        "The enrollment is already completed",
        ErrorType.Validation);
}
