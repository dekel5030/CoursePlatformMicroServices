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
}
