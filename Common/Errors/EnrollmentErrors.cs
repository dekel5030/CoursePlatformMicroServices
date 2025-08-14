namespace Common.Errors;

public static partial class EnrollmentErrors
{
    public static readonly Error EnrollmentNotFound = new("EnrollmentNotFound", 404);
    public static readonly Error EnrollmentAlreadyExists = new("EnrollmentAlreadyExists", 409);
}