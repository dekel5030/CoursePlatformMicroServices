using Kernel;

namespace Domain.Users.Errors;

public static class UserErrors
{
    public static readonly Error NotFound = Error.NotFound("UserService.User.NotFound", "The user was not found.");
    public static readonly Error EmailAlreadyExists = Error.Conflict("UserService.User.EmailAlreadyExists", "The email is already in use by another user.");
    public static readonly Error Unauthorized = Error.Unauthorized("UserService.User.Unauthorized", "You are not authorized to perform this action.");
    public static readonly Error Forbidden = Error.Forbidden("UserService.User.Forbidden", "You do not have permission to access this resource.");
}