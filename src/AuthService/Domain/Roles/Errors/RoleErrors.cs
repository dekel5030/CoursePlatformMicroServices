using Kernel;

namespace Auth.Domain.Roles.Errors;

public static class RoleErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Role.NotFound",
        "Role was not found");

    public static readonly Error DuplicateName = Error.Conflict(
        "Role.DuplicateName",
        "Role with this name already exists");

    public static readonly Error NameCannotBeEmpty = Error.Validation(
        "Role.NameCannotBeEmpty",
        "Role name cannot be empty");

    internal static readonly Error InvalidPermissionEffect = Error.Validation(
        "Role.InvalidPermissionEffect",
        "Role permission effect must be 'Allow'");
}
