using System.Security.Claims;
using Auth.Application.Abstractions.Auth;
using Microsoft.AspNetCore.Http;

namespace Auth.Infrastructure.Auth.Context;

internal sealed class ExternalUserContext(IHttpContextAccessor httpContextAccessor)
        : IExternalUserContext
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public string IdentityId => User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User?.FindFirstValue("sub")
        ?? string.Empty;

    public string Email => User?.FindFirstValue(ClaimTypes.Email)
        ?? User?.FindFirstValue("email")
        ?? string.Empty;

    public string FirstName => User?.FindFirstValue(ClaimTypes.GivenName)
        ?? User?.FindFirstValue("given_name")
        ?? string.Empty;

    public string LastName => User?.FindFirstValue(ClaimTypes.Surname)
        ?? User?.FindFirstValue("family_name")
        ?? string.Empty;

    public DateTime ExpiryUtc
    {
        get
        {
            string expClaim = User?.FindFirstValue("exp");
            if (expClaim != null && long.TryParse(expClaim, out long unixExp))
            {
                return DateTimeOffset.FromUnixTimeSeconds(unixExp).UtcDateTime;
            }

            return DateTime.UtcNow.AddMinutes(5);
        }
    }
}
