namespace Auth.Api.Endpoints.Auth;

internal sealed class OpenIdConfigurationEndpoint : IEndpoint
{
    internal static readonly string[] _value = new[] { "id_token" };
    internal static readonly string[] _valueArray = new[] { "RS256" };

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/.well-known/openid-configuration", (HttpContext context) =>
        {
            string baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";

            return Results.Ok(new
            {
                issuer = "course-platform-auth",
                jwks_uri = $"{baseUrl}/.well-known/jwks.json",
                response_types_supported = _value,
                id_token_signing_alg_values_supported = _valueArray
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
