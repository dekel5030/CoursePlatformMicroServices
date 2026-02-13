using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;
using Courses.Domain.Users;

namespace Courses.Application.Features.Shared.Loaders;

public sealed record CoursePageData(
    Course Course,
    List<Module> Modules,
    List<Lesson> Lessons,
    User? Instructor,
    Category? Category);
