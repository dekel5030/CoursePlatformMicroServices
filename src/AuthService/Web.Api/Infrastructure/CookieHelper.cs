using Application.Abstractions.Security;

namespace Web.Api.Infrastructure;

public static class CookieHelper
{
    private const string AccessTokenCookieName = "accessToken";
    private const string RefreshTokenCookieName = "refreshToken";

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
        context.Response.Cookies.Append(AccessTokenCookieName, accessToken, accessTokenOptions);

        // Set refresh token cookie (longer expiration)
        var refreshTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = refreshTokenExpiresAt,
            Path = "/"
        };
        context.Response.Cookies.Append(RefreshTokenCookieName, refreshToken, refreshTokenOptions);
    }

    public static string? GetRefreshTokenFromCookie(HttpContext context)
    {
        return context.Request.Cookies[RefreshTokenCookieName];
    }

    public static void ClearAuthCookies(HttpContext context)
    {
        context.Response.Cookies.Delete(AccessTokenCookieName);
        context.Response.Cookies.Delete(RefreshTokenCookieName);
    }
}
