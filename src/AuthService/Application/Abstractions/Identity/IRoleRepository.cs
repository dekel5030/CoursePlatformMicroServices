using Domain.Roles;
using Domain.Roles.Primitives;
using Kernel;

namespace Application.Abstractions.Identity;

public interface IRoleRepository<TRole> where TRole : Role
{
    Task<Result> AddRoleAsync(TRole role, CancellationToken ct = default);
    Task<Result> UpdateRoleAsync(TRole role, CancellationToken ct = default);
    Task<Role?> GetByIdAsync(Guid roleId, CancellationToken ct = default);
    IQueryable<TRole> Roles { get; }
}
