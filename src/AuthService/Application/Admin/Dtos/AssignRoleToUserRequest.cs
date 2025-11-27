namespace Application.Admin.Dtos;

public record AssignRoleToUserRequest(Guid UserId, int RoleId);
