using Domain.AuthUsers;
using Kernel;

namespace Application.Abstractions.Identity;

public interface IUserManager<TUser> where TUser : AuthUser
{
    Task<Result> AddToRoleAsync(TUser user, string defaultRole);
    Task<Result> CreateAsync(TUser user, string password);
    Task<TUser?> FindByEmailAsync(string email);
    Task<TUser?> FindByIdAsync(string userId);
}
