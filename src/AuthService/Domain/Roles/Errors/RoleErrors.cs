using Kernel;

namespace Domain.Roles.Errors;

public static class RoleErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Role.NotFound",
        "Role was not found");

    public static readonly Error DuplicateName = Error.Conflict(
        "Role.DuplicateName",
        "Role with this name already exists");
}
