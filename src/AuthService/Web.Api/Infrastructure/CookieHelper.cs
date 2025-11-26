using Application.Abstractions.Security;
using Application.AuthUsers.Dtos;

namespace Auth.Api.Infrastructure;

public static class CookieHelper
{
    private const string _refreshTokenCookieName = "refreshToken";

    public static void SetRefreshTokenCookie(
        HttpContext context,
        AuthTokensResult authTokensResult)
    {
        var refreshTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = authTokensResult.RefreshTokenExpiresAt,
            Path = "/"
        };
        context.Response.Cookies.Append(
            _refreshTokenCookieName, 
            authTokensResult.RefreshToken, 
            refreshTokenOptions);
    }

    public static string? GetRefreshTokenFromCookie(HttpContext context)
    {
        return context.Request.Cookies[_refreshTokenCookieName];
    }

    public static void ClearRefreshCookies(HttpContext context)
    {
        context.Response.Cookies.Delete(_refreshTokenCookieName);
    }
}
