namespace Application.Admin.Dtos;

public record AssignPermissionToUserRequest(Guid UserId, int PermissionId);
