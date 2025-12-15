namespace Gateway.Api.Models;

public class UserPermissionsDto
{
    public HashSet<string> Permissions { get; set; } = new();

    public HashSet<string> Roles { get; set; } = new();
}