using Kernel;
using Kernel.Auth.AuthTypes;

namespace Auth.Domain.Permissions.Errors;

public class PermissionErrors
{
    public static readonly Error PermissionNotAssigned = Error.Conflict("Permission.NotAssigned", "The permission is not assigned to the role.");

    public static readonly Error PermissionAlreadyAssigned = Error.Conflict("Permission.AlreadyAssigned", "The permission is already assigned to the role.");
    public static readonly Error InvalidEffect = Error.Conflict("Permission.InvalidEffect", $"The effect is invalid. allowed types {string.Join(", ", Enum.GetNames(typeof(EffectType)))}");
    public static readonly Error InvalidAction = Error.Conflict("Permission.InvalidAction", $"The action is invalid. allowed types {string.Join(", ", Enum.GetNames(typeof(ActionType)))}");
    public static readonly Error InvalidResource = Error.Conflict("Permission.InvalidResource", $"The resource is invalid. Allowed resource types: {string.Join(", ", Enum.GetNames(typeof(ResourceType)))}");
}
