using Courses.Domain.Categories.Primitives;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;

namespace Courses.Domain.Courses;

public interface ICourseSnapshot
{
    CourseId Id { get; }
    Title Title { get; }
    Description Description { get; }
    CourseStatus Status { get; }
    DifficultyLevel Difficulty { get; }
    Money Price { get; }
    Language Language { get; }
    Slug Slug { get; }
    UserId InstructorId { get; }
    CategoryId CategoryId { get; }
    IReadOnlyCollection<Tag> Tags { get; }
    IReadOnlyCollection<ImageUrl> Images { get; }
}
