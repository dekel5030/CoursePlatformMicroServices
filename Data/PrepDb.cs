using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data
{
    public static class PrepDb
    {
        public static async Task PopulateAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<UsersDbContext>();

                await context.Database.MigrateAsync();

                if (!context.Users.Any())
                {
                    await SeedData(context);
                }
                else
                {
                    Console.WriteLine("--> Users table already populated.");
                }
            }
        }

        private static async Task SeedData(UsersDbContext context)
        {
            Console.WriteLine("--> Seeding data...");

            var users = new List<User>
            {
                // new User
                // {
                //     FullName = "John Doe",
                //     Email = "johndoe@gmail.com",
                //     Bio = "Passionate software developer.",
                //     DateOfBirth = new DateTime(1990, 5, 21),
                //     PhoneNumber = "050-1234567"
                // },
                // new User
                // {
                //     FullName = "Jane Smith",
                //     Email = "janesmith@example.com",
                //     Bio = "UX/UI designer and coffee lover.",
                //     DateOfBirth = new DateTime(1988, 11, 3),
                //     PhoneNumber = "052-9876543"
                // },
                // new User
                // {
                //     FullName = "Lior Cohen",
                //     Email = "liorcohen@startup.io",
                //     Bio = "Tech entrepreneur and public speaker.",
                //     DateOfBirth = new DateTime(1985, 3, 14)
                // },
                // new User
                // {
                //     FullName = "Maya Levi",
                //     Email = "maya.levi@outlook.com",
                //     Bio = "Full-stack developer specializing in .NET and React.",
                //     DateOfBirth = new DateTime(1992, 7, 30),
                //     PhoneNumber = "053-2223344"
                // }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}