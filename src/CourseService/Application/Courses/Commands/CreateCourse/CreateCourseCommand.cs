using Courses.Domain.Categories;
using Courses.Domain.Shared.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.CreateCourse;

public sealed record CreateCourseCommand(
    CategoryId CategoryId,
    Title? Title,
    Description? Description) : ICommand<CreateCourseResponse>;
