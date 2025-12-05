using Domain.AuthUsers;
using Kernel;

namespace Application.Abstractions.Identity;

public interface ISignInManager<TUser>
    where TUser : AuthUser
{
    IUserManager<TUser> UserManager { get; }
    Task<Result> PasswordSignInAsync(
        TUser user,
        string password,
        bool isPersistent,
        bool lockoutOnFailure);
    Task SignInAsync(AuthUser user, bool isPersistent);
    Task SignOutAsync();
}
