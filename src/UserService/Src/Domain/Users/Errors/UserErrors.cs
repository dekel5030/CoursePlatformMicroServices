using Kernel;

namespace Domain.Users.Errors;

public static class UserErrors
{
    public static readonly Error NotFound = Error.NotFound("UserService.User.NotFound", "The user was not found.");
}
