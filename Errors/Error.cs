namespace Common.Errors;

public sealed record Error(string MessageKey, int HttpStatus, bool IsPublic = true)
{
    public static readonly Error DuplicateEmail = new("DuplicateEmail", 409);
    public static readonly Error DatabaseError = new("DatabaseError", 500, IsPublic: false);
    public static readonly Error UserNotFound = new("UserNotFound", 404);
    public static readonly Error InvalidPassword = new("InvalidPassword", 400);
    public static readonly Error Unexpected = new("Unexpected", 500);
    public static readonly Error EmailNotConfirmed = new("EmailNotConfirmed", 403);
    public static readonly Error InvalidPageNumber = new("InvalidPageNumber", 400);
    public static readonly Error InvalidPageSize = new("InvalidPageSize", 400);
    public static readonly Error InvalidCredentials = new("InvalidCredentials", 401);
    public static readonly Error UnAuthenticated = new("UnAuthenticated", 401);
}