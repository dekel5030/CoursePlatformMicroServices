using Kernel;

namespace Domain.AuthUsers.Errors;

public static class AuthUserErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "AuthUser.NotFound",
        "Auth user was not found");

    public static readonly Error DuplicateEmail = Error.Conflict(
        "AuthUser.DuplicateEmail",
        "Email is already in use");

    public static readonly Error InvalidCredentials = Error.Validation(
        "AuthUser.InvalidCredentials",
        "Invalid email or password");

    public static readonly Error AccountLocked = Error.Validation(
        "AuthUser.AccountLocked",
        "Account is locked");

    public static readonly Error AccountInactive = Error.Validation(
        "AuthUser.AccountInactive",
        "Account is inactive");

    public static readonly Error UserLockedOut = Error.Validation(
        "AuthUser.UserLockedOut",
        "User account is locked out");

    public static readonly Error EmailNotConfirmed = Error.Validation(
        "AuthUser.EmailNotConfirmed",
        "Email address is not confirmed");

    public static readonly Error Unauthorized = Error.Unauthorized(
        "AuthUser.Unauthorized",
        "Unauthorized access to the resource");

    public static readonly Error IsLockOut = Error.Validation(
        "AuthUser.IsLockOut",
        "User account is locked out due to multiple failed login attempts.");

    public static readonly Error Required2FA = Error.Validation(
        "AuthUser.Required2FA",
        "Two-factor authentication is required to sign in.");

    public static readonly Error RoleAlreadyExists = Error.Conflict(
        "AuthUser.RoleAlreadyExists",
        "User already has this role assigned.");

    public static readonly Error UserOrEmailIsTaken = Error.Conflict(
        "AuthUser.UserOrEmailIsTaken",
        "User with this email or username already exists.");

    internal static readonly Error PermissionAlreadyExists = Error.Conflict(
        "AuthUser.PermissionAlreadyExists",
        "User already has this permission assigned.");

    internal static Error PermissionAlreadyExistsWithValue(string permissionName) =>
        Error.Conflict(
            "User.PermissionExists",
            $"The permission '{permissionName}' already exists for this user."
        );
}
