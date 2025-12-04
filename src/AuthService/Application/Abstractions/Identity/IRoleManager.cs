using Domain.Roles;
using Kernel;

namespace Application.Abstractions.Identity;

public interface IRoleManager<TRole> where TRole : Role
{
    Task<Result> CreateAsync(TRole role);
}
