namespace Application.Admin.Dtos;

public record RoleDto(int Id, string Name, IEnumerable<PermissionDto> Permissions);
