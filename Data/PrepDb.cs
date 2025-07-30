using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

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

        var credentials = new List<UserCredentials>
        {
            new UserCredentials { UserId = 1, PasswordHash = "123", Email = "test1@gmail.com"},
            new UserCredentials { UserId = 2, PasswordHash = "312", Email = "test2@gmail.com"},
            new UserCredentials { UserId = 3, PasswordHash = "123", Email = "test3@gmail.com"}
        };

        await dbContext.UserCredentials.AddRangeAsync(credentials);
        await dbContext.SaveChangesAsync();
    }
}