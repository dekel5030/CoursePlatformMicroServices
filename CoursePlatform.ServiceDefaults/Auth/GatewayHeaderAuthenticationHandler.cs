using Kernel.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace CoursePlatform.ServiceDefaults.Auth;

public class GatewayHeaderAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "GatewayHeaderScheme";

    public GatewayHeaderAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(HeaderNames.UserIdHeader, out StringValues userIdValues))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var userId = userIdValues.FirstOrDefault();
        if (string.IsNullOrEmpty(userId))
        {
            return Task.FromResult(AuthenticateResult.Fail("User ID header is empty."));
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim("sub", userId),
        };

        if (Request.Headers.TryGetValue(HeaderNames.UserRoleHeader, out StringValues roleValues))
        {
            foreach (var role in roleValues.Where(r => !string.IsNullOrEmpty(r)))
            {
                claims.Add(new Claim(ClaimTypes.Role, role!));
            }
        }

        if (Request.Headers.TryGetValue(HeaderNames.UserPermissionsHeader, out StringValues permissionValues))
        {
            foreach (var permission in permissionValues.Where(p => !string.IsNullOrEmpty(p)))
            {
                if (PermissionClaim.TryParse(permission!, out var parsedPermission))
                {
                    claims.Add(parsedPermission);
                }
            }
        }

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}