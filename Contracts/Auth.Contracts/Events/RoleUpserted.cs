using SharedKernel;

namespace Domain.Roles;

public record RoleUpserted(Guid RoleId, string RoleName) : IDomainEvent;