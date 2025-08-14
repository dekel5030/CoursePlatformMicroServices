namespace Common.Resources.ValidationMessages
{
    public class ValidationMessages
    {
        public const string EmailRequired = "EmailRequired";
        public const string EmailInvalidFormat = "EmailInvalidFormat";
        public const string EmailTooLong = "EmailTooLong";

        public const string PasswordRequired = "PasswordRequired";
        public const string PasswordTooShort = "PasswordTooShort";
        public const string PasswordTooLong = "PasswordTooLong";
        public const string PasswordMismatch = "PasswordMismatch";
        public const string PasswordMustContainUppercase = "PasswordMustContainUppercase";
        public const string PasswordMustContainLowercase = "PasswordMustContainLowercase";
        public const string PasswordMustContainDigit = "PasswordMustContainDigit";
        public const string PasswordMustContainSpecial = "PasswordMustContainSpecial";

        public const string ConfirmPasswordRequired = "ConfirmPasswordRequired";
        public const string ConfirmPasswordTooLong = "ConfirmPasswordTooLong";

        public const string FullNameRequired = "FullNameRequired";
        public const string FullNameTooLong = "FullNameTooLong";

        public const string PageNumberGreaterThanZero = "PageNumberGreaterThanZero";
        public const string PageSizeGreaterThanZero = "PageSizeGreaterThanZero";

        public const string PermissionNameRequired = "PermissionNameRequired";

        public const string RoleNameRequired = "RoleNameRequired";

        // === Enrollment Validations ===
        public const string CourseIdRequired = "CourseIdRequired";
        public const string UserIdRequired = "UserIdRequired";
        public const string InvalidEnrollmentStatus = "InvalidEnrollmentStatus";
        public const string ExpirationMustBeInFuture = "ExpirationMustBeInFuture";
        public const string PageSizeLessThanOrEqualTo = "PageSizeLessThanOrEqualTo";
        public const string EnrollmentIdGreaterThanZero = "EnrollmentIdGreaterThanZero";
        public const string CourseIdGreaterThanZero = "CourseIdGreaterThanZero";
        public const string UserIdGreaterThanZero = "UserIdGreaterThanZero";
    }
}