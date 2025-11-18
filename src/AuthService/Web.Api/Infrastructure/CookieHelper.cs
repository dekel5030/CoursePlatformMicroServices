using Application.Abstractions.Security;

namespace Web.Api.Infrastructure;

public static class CookieHelper
{
    private const string _accessTokenCookieName = "accessToken";
    private const string _refreshTokenCookieName = "refreshToken";

    public static void SetAuthCookies(
        HttpContext context,
        ITokenService tokenService,
        string email,
        IEnumerable<string> roles,
        IEnumerable<string> permissions,
        string refreshToken,
        DateTime refreshTokenExpiresAt)
    {
        // Generate access token
        var tokenRequest = new TokenRequestDto
        {
            Email = email,
            Roles = roles,
            Permissions = permissions
        };
        var accessToken = tokenService.GenerateToken(tokenRequest);

        // Set access token cookie (shorter expiration)
        var accessTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(60), // Same as JWT expiration
            Path = "/"
        };
        context.Response.Cookies.Append(_accessTokenCookieName, accessToken, accessTokenOptions);

        // Set refresh token cookie (longer expiration)
        var refreshTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = refreshTokenExpiresAt,
            Path = "/"
        };
        context.Response.Cookies.Append(_refreshTokenCookieName, refreshToken, refreshTokenOptions);
    }

    public static string? GetRefreshTokenFromCookie(HttpContext context)
    {
        return context.Request.Cookies[_refreshTokenCookieName];
    }

    public static void ClearAuthCookies(HttpContext context)
    {
        context.Response.Cookies.Delete(_accessTokenCookieName);
        context.Response.Cookies.Delete(_refreshTokenCookieName);
    }
}
