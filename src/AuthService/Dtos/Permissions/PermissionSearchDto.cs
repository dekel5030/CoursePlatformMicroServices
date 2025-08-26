namespace AuthService.Dtos.Permissions;

public class PermissionSearchDto
{
    public string? Name { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
