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

            if (!await dbContext.UserCredentials.AnyAsync())
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
        Console.WriteLine("--> Seeding data...");

        var roles = new List<Role>
        {
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Instructor" },
            new Role { Id = 3, Name = "Student" }
        };

        var userRoles = new List<UserRole>
        {
            new UserRole { UserId = 1, RoleId = 1 }, 
            new UserRole { UserId = 2, RoleId = 2 }, 
            new UserRole { UserId = 3, RoleId = 3 } 
        };

        await dbContext.Roles.AddRangeAsync(roles);
        await dbContext.UserRoles.AddRangeAsync(userRoles);
        
        await dbContext.SaveChangesAsync();
    }
}