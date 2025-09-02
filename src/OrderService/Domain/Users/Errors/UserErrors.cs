using Kernel;

namespace Domain.Users.Errors;

public static class UserErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Orders.User.NotFound", "User not found");
}