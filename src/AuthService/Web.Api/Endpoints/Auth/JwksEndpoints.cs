using Auth.Infrastructure.Auth;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Api.Endpoints.Auth;

internal sealed class JwksEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/.well-known/jwks.json", (KeyManager keyManager) =>
        {
            var publicKey = keyManager.GetPublicKey();

            var jwk = JsonWebKeyConverter.ConvertFromSecurityKey(publicKey);

            jwk.Kid = KeyManager.KeyId;
            jwk.Use = "sig";
            jwk.Alg = "RS256";

            return Results.Ok(new { keys = new[] { jwk } });
        })
        .AllowAnonymous()
        .WithTags(Tags.Auth)
        .WithName("GetJwks")
        .WithSummary("Get JSON Web Key Set")
        .WithDescription("Retrieves the public keys used for token signature verification")
        .Produces<object>(StatusCodes.Status200OK);
    }
}
