using System.Security.Claims;

namespace Kernel.Auth;

public static class CoursePlatformClaims
{
    public const string UserId = "sub";
    public const string Email = "email";
    public const string FirstName = "given_name";
    public const string LastName = "family_name";
    public const string Role = "role";
    public const string Permission = "cp_p";
    public const string IdentityId = "ext_id";

    public static Claim CreateUserIdClaim(Guid userId)
        => new Claim(UserId, userId.ToString());
    
    public static Claim CreateEmailClaim(string email)
        => new Claim(Email, email);

    public static Claim CreateFirstNameClaim(string firstName)
        => new Claim(FirstName, firstName);

    public static Claim CreateLastNameClaim(string lastName)
        => new Claim(LastName, lastName);

    public static Claim CreateRoleClaim(string role)
        => new Claim(Role, role);

    public static Claim CreatePermissionClaim(string value)
        => new Claim(Permission, value);

    public static Claim CreateIdentityIdClaim(string identityId)
        => new Claim(IdentityId, identityId);

    public static IEnumerable<string> GetPermissions(this ClaimsPrincipal user)
        => user.FindAll(Permission).Select(c => c.Value);

    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        string? value = user.FindFirst(UserId)?.Value;
        return Guid.TryParse(value, out Guid id) ? id : null;
    }

    public static string? GetEmail(this ClaimsPrincipal user)
        => user.FindFirst(Email)?.Value;
}
