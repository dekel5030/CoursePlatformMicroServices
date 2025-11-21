using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Application.AuthUsers.Commands.LoginUser;
using Application.AuthUsers.Dtos;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth;

public class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/login", async (
            LoginRequestDto request,
            ICommandHandler<LoginUserCommand, AuthTokensDto> handler,
            ITokenService tokenService,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginUserCommand(request);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: tokensDto =>
                {
                    // Set cookies with tokens
                    CookieHelper.SetAuthCookies(
                        httpContext,
                        tokenService,
                        tokensDto.Email,
                        tokensDto.Roles,
                        tokensDto.Permissions,
                        tokensDto.RefreshToken,
                        tokensDto.RefreshTokenExpiresAt);
                    
                    // Return public response without tokens
                    var response = new AuthResponseDto
                    {
                        AuthUserId = tokensDto.AuthUserId,
                        UserId = tokensDto.AuthUserId,
                        Email = tokensDto.Email,
                        Roles = tokensDto.Roles,
                        Permissions = tokensDto.Permissions,
                        Message = "Login successful"
                    };
                    
                    return Results.Ok(response);
                },
                onFailure: CustomResults.Problem);
        })
        .WithTags(Tags.Auth)
        .WithName("Login")
        .WithSummary("Login user")
        .WithDescription("Authenticates a user and returns authentication tokens in HttpOnly cookies")
        .Produces<AuthResponseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
