using Courses.Domain.Categories;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.PatchCourse;

public record PatchCourseCommand(
    CourseId CourseId,
    Title? Title,
    Description? Description,
    Money? Price,
    DifficultyLevel? Difficulty,
    CategoryId? CategoryId,
    Language? Language,
    IReadOnlyList<string>? Tags,
    string? Slug) : ICommand;
