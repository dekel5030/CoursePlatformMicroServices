using CourseService.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<CourseDbContext>();

        context.Database.Migrate();

        if (!context.Courses.Any())
        {
            SeedData(context);
        }
    }

    private static void SeedData(CourseDbContext context)
    {
        Console.WriteLine("--> Seeding Data...");

        var courses = new List<Course>
        {
            new Course { Title = "C# for Beginners", Description = "Learn the basics of C# programming", IsPublished = true },
            new Course { Title = "Advanced .NET", Description = "Deep dive into .NET ecosystem", IsPublished = false },
            new Course { Title = "ASP.NET Core Web API", Description = "Build RESTful APIs with ASP.NET Core", IsPublished = true },
            new Course { Title = "Entity Framework Core", Description = "Master EF Core for data access", IsPublished = true },
            new Course { Title = "Blazor Fundamentals", Description = "Learn to build interactive web UIs with Blazor", IsPublished = false }
        };

        context.Courses.AddRange(courses);
        context.SaveChanges();

        // עכשיו נוסיף שיעורים לכל קורס
        var lessons = new List<Lesson>
        {
            // לשיעורים של C# for Beginners
            new Lesson { CourseId = courses[0].Id, Title = "Introduction to C#", Description = "Overview of C# language", IsPreview = true, Order = 1 },
            new Lesson { CourseId = courses[0].Id, Title = "Variables and Data Types", Description = "Understanding variables", IsPreview = true, Order = 2 },
            new Lesson { CourseId = courses[0].Id, Title = "Control Structures", Description = "If, loops and more", IsPreview = false, Order = 3 },

            // לשיעורים של Advanced .NET
            new Lesson { CourseId = courses[1].Id, Title = ".NET Core Overview", Description = "What is .NET Core?", IsPreview = true, Order = 1 },
            new Lesson { CourseId = courses[1].Id, Title = "Dependency Injection", Description = "Using DI in .NET", IsPreview = false, Order = 2 },
            new Lesson { CourseId = courses[1].Id, Title = "Middleware", Description = "Understanding middleware pipeline", IsPreview = false, Order = 3 },

            // לשיעורים של ASP.NET Core Web API
            new Lesson { CourseId = courses[2].Id, Title = "REST Principles", Description = "Introduction to REST APIs", IsPreview = true, Order = 1 },
            new Lesson { CourseId = courses[2].Id, Title = "Creating Controllers", Description = "Build your first controller", IsPreview = false, Order = 2 },

            // לשיעורים של EF Core
            new Lesson { CourseId = courses[3].Id, Title = "Getting Started with EF Core", Description = "Setup and basics", IsPreview = true, Order = 1 },
            new Lesson { CourseId = courses[3].Id, Title = "Migrations", Description = "Manage database schema", IsPreview = false, Order = 2 },

            // לשיעורים של Blazor
            new Lesson { CourseId = courses[4].Id, Title = "Introduction to Blazor", Description = "What is Blazor?", IsPreview = true, Order = 1 },
            new Lesson { CourseId = courses[4].Id, Title = "Components Basics", Description = "Creating and using components", IsPreview = false, Order = 2 }
        };

        context.Lessons.AddRange(lessons);
        context.SaveChanges();
    }
}