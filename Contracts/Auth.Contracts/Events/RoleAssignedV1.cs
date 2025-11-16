namespace Auth.Contracts.Events;

public sealed record RoleAssignedV1(
    int AuthUserId,
    int RoleId,
    string RoleName,
    DateTime AssignedAt
);
