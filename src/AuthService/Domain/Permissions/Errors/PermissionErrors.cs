using Kernel;

namespace Domain.Permissions.Errors;

public class PermissionErrors
{
    internal static readonly Error PermissionNotAssigned = Error.Conflict("Permission.NotAssigned", "The permission is not assigned to the role.");

    public static Error PermissionAlreadyAssigned => Error.Conflict("Permission.AlreadyAssigned", "The permission is already assigned to the role.");
}
