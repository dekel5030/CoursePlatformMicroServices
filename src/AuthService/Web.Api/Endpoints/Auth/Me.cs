using System.Security.Claims;
using Application.Abstractions.Data;
using Application.AuthUsers.Dtos;
using Microsoft.EntityFrameworkCore;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth;

public class Me : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("auth/me", async (
            HttpContext httpContext,
            IReadDbContext readDbContext,
            CancellationToken cancellationToken) =>
        {
            // Get email from JWT claims
            var email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value 
                        ?? httpContext.User.FindFirst("email")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return Results.Unauthorized();
            }

            // Fetch user from database with all necessary navigation properties
            var authUser = await readDbContext.AuthUsers
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .Include(u => u.UserPermissions)
                    .ThenInclude(up => up.Permission)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (authUser == null)
            {
                return Results.NotFound(new { message = "User not found" });
            }

            // Extract roles and permissions
            var roles = authUser.UserRoles
                .Select(ur => ur.Role.Name)
                .Distinct()
                .ToList();

            var permissionsFromRoles = authUser.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Name);

            var directPermissions = authUser.UserPermissions
                .Select(up => up.Permission.Name);

            var allPermissions = permissionsFromRoles
                .Concat(directPermissions)
                .Distinct()
                .ToList();

            var response = new AuthResponseDto
            {
                AuthUserId = authUser.Id.Value,
                UserId = authUser.Id.Value, // Unified ID
                Email = authUser.Email,
                Roles = roles,
                Permissions = allPermissions
            };

            return Results.Ok(response);
        })
        .RequireAuthorization() // This endpoint requires authentication
        .WithTags(Tags.Auth)
        .WithName("GetCurrentUser")
        .WithSummary("Get current authenticated user")
        .WithDescription("Returns the current authenticated user's information from JWT token")
        .Produces<AuthResponseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
