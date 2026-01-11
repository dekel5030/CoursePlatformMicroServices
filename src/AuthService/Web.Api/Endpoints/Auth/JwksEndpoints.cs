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
            var parameters = publicKey.Rsa.ExportParameters(false);

            var jwk = new
            {
                keys = new[]
                {
                    new
                    {
                        kty = "RSA",
                        use = "sig",
                        kid = KeyManager.KeyId,
                        n = Base64UrlEncoder.Encode(parameters.Modulus),
                        e = Base64UrlEncoder.Encode(parameters.Exponent),
                        alg = "RS256"
                    }
                }
            };

            return Results.Json(jwk);
        })
        .AllowAnonymous()
        .WithTags(Tags.Auth)
        .WithName("GetJwks")
        .WithSummary("Get JSON Web Key Set")
        .WithDescription("Retrieves the public keys used for token signature verification")
        .Produces<object>(StatusCodes.Status200OK);
    }
}