namespace Application.AuthUsers.Dtos;

/// <summary>
/// Internal DTO used within the application layer to pass token information
/// This includes the refresh token which should not be exposed in the API response body
/// </summary>
public class AuthTokensResult
{
    public required Guid AuthUserId { get; set; } 
    public required string Email { get; set; }
    public required IEnumerable<string> Roles { get; set; }
    public required IEnumerable<string> Permissions { get; set; }

    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
}

