using AuthService.Data.Context;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

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

        var permissions = new List<Permission>
        {
            new Permission { Id = 1, Name = "ViewCourses" },
            new Permission { Id = 2, Name = "EnrollInCourses" },
            new Permission { Id = 3, Name = "CommentOnLessons" }
        };

        var rolePermissions = new List<RolePermission>
        {
            new RolePermission { RoleId = 3, PermissionId = 1 }, 
            new RolePermission { RoleId = 3, PermissionId = 2 }, 
            new RolePermission { RoleId = 3, PermissionId = 3 }  
        };

        var userRoles = new List<UserRole>
        {
            new UserRole { UserId = 1, RoleId = 1 },
            new UserRole { UserId = 2, RoleId = 2 },
            new UserRole { UserId = 3, RoleId = 3 }
        };

        await dbContext.Roles.AddRangeAsync(roles);
        await dbContext.Permissions.AddRangeAsync(permissions);
        await dbContext.RolePermissions.AddRangeAsync(rolePermissions);
        await dbContext.UserRoles.AddRangeAsync(userRoles);

        await dbContext.SaveChangesAsync();

        Console.WriteLine("--> Seeding completed.");
    }
}