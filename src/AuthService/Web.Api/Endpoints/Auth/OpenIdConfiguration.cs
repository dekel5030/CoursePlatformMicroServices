namespace Auth.Api.Endpoints.Auth;

public class OpenIdConfigurationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/.well-known/openid-configuration", (HttpContext context) =>
        {
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";

            return Results.Ok(new
            {
                issuer = "course-platform-auth",
                jwks_uri = $"{baseUrl}/.well-known/jwks.json",
                response_types_supported = new[] { "id_token" },
                id_token_signing_alg_values_supported = new[] { "RS256" }
            });
        })
        .AllowAnonymous()
        .WithTags(Tags.Auth)
        .WithName("GetOpenIdConfiguration")
        .WithSummary("Get OpenID configuration")
        .WithDescription("Retrieves the OpenID Connect discovery document for authentication configuration")
        .Produces<object>(StatusCodes.Status200OK);
    }
}