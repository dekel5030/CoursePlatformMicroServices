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
}
