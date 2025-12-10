using Domain.AuthUsers;
using Kernel;

namespace Application.Abstractions.Identity;

public interface ISignService<TUser>
    where TUser : AuthUser
{
    Task<Result> PasswordSignInAsync(
        TUser user,
        string password,
        bool isPersistent,
        bool lockoutOnFailure);
    Task SignInAsync(TUser user, bool isPersistent);
    Task SignOutAsync();
    Task<Result> RegisterAsync(TUser user, string password);
}
