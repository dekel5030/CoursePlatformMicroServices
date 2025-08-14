using Microsoft.AspNetCore.Authorization;

namespace Common.Auth.Attributes;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(PermissionType permission) : base(policy: permission.ToString())
    {
        
    }
}