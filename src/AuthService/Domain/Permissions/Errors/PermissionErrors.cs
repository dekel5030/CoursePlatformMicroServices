using Kernel;

namespace Domain.Permissions.Errors;

public class PermissionErrors
{
    public static Error PermissionAlreadyAssigned => Error.Conflict("Permission.AlreadyAssigned", "The permission is already assigned to the role.");
}
