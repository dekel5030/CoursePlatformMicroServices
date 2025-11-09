namespace Application.Courses.Commands.CreateCourse;

public record CreateCourseDto(
    string? Title,
    string? Description,
    string? InstructorId,
    string? ImageUrl,
    decimal? PriceAmount,
    string? PriceCurrency
);