using Application.Abstractions.Messaging;
using Application.AuthUsers.Commands.LoginUser;
using Application.AuthUsers.Dtos;
using Auth.Api.Extensions;
using Auth.Api.Infrastructure;
using Kernel;

namespace Auth.Api.Endpoints.Auth;

public class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/login", async (
            LoginRequestDto request,
            ICommandHandler<LoginUserCommand, AuthTokensResult> handler,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginUserCommand(request);

            Result<AuthTokensResult> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: authTokens =>
                {
                    CookieHelper.SetRefreshTokenCookie(
                        httpContext,
                        authTokens);
                    
                    var response = new AuthResponseDto
                    {
                        AuthUserId = authTokens.AuthUserId,
                        UserId = authTokens.AuthUserId,
                        Email = authTokens.Email,
                        Roles = authTokens.Roles,
                        Permissions = authTokens.Permissions,
                        AccessToken = authTokens.AccessToken,
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