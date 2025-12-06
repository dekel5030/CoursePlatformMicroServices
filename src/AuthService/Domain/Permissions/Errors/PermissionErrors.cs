using Kernel;

namespace Domain.Permissions.Errors;

public class PermissionErrors
{
    public static readonly Error PermissionNotAssigned = Error.Conflict("Permission.NotAssigned", "The permission is not assigned to the role.");

    public static readonly Error PermissionAlreadyAssigned = Error.Conflict("Permission.AlreadyAssigned", "The permission is already assigned to the role.");
}
