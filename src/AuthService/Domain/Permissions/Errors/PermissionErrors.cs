using Kernel;

namespace Domain.Permissions.Errors;

public class PermissionErrors
{
    public static readonly Error PermissionNotAssigned = Error.Conflict("Permission.NotAssigned", "The permission is not assigned to the role.");

    public static readonly Error PermissionAlreadyAssigned = Error.Conflict("Permission.AlreadyAssigned", "The permission is already assigned to the role.");
    public static readonly Error InvalidAction = Error.Conflict("Permission.InvalidAction", "The action is invalid.");
    public static readonly Error InvalidResource = Error.Conflict("Permission.InvalidResource", "The resource is invalid.");
}
