namespace Common.Errors;

public static partial class AuthErrors
{
    public static readonly Error UnAuthorized = new("Unauthorized", 401);
    public static readonly Error PermissionNotFound = new("PermissionNotFound", 404);
    public static readonly Error PermissionAlreadyExists = new("PermissionAlreadyExists", 409);
    public static readonly Error OneOrMorePermissionsNotFound = new("OneOrMorePermissionsNotFound", 404);
    public static readonly Error RoleNotFound = new("RoleNotFound", 404);
    public static readonly Error RoleAlreadyExists = new("RoleAlreadyExists", 409);
    public static readonly Error UserNotFound = new("UserNotFound", 404);
    
}