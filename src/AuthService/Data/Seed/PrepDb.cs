using AuthService.Data.Context;
using AuthService.Models;
using Common.Auth;
using Microsoft.EntityFrameworkCore;
using UserRole = AuthService.Models.UserRole;

namespace AuthService.Data.Seed;

public static class PrepDb
{
    public static async Task InitializeAsync(IApplicationBuilder applicationBuilder)
    {
        using (var scope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

            Console.WriteLine("--> Applying migrations...");
            await dbContext.Database.MigrateAsync();

            if (!await dbContext.Permissions.AnyAsync())
            {
                await SeedData(dbContext);
            }
            else
            {
                Console.WriteLine("--> AuthDb is already populated.");
            }
        }
    }

    private static async Task SeedData(AuthDbContext dbContext)
    {
        dbContext.UserRoles.RemoveRange(dbContext.UserRoles);
        dbContext.RolePermissions.RemoveRange(dbContext.RolePermissions);
        dbContext.Roles.RemoveRange(dbContext.Roles);
        dbContext.Permissions.RemoveRange(dbContext.Permissions);
        Console.WriteLine("--> Seeding data...");

        var roles = new List<Role>
        {
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Instructor" },
            new Role { Id = 3, Name = "User" }
        };

        var permissionNames = new List<string>();
        foreach (var permissionName in Enum.GetValues<PermissionType>())
        {
            permissionNames.Add(permissionName.ToString());
        }

        var permissions = permissionNames.Select(name => new Permission
            {
                Name = name,
            }).ToList();
        
        await dbContext.Permissions.AddRangeAsync(permissions);
        await dbContext.SaveChangesAsync();

        permissions = await dbContext.Permissions.ToListAsync();

        var rolePermissions = new List<RolePermission>();

        foreach (var permission in permissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = 1, // Admin role
                PermissionId = permission.Id
            });
        }


        await dbContext.Roles.AddRangeAsync(roles);
        await dbContext.RolePermissions.AddRangeAsync(rolePermissions);

        dbContext.UserRoles.AddRange(new List<UserRole>
        {
            new UserRole { UserId = 1, RoleId = 1 }, // Admin

        });
        
        await dbContext.SaveChangesAsync();

        Console.WriteLine("--> Seeding completed.");
    }
}