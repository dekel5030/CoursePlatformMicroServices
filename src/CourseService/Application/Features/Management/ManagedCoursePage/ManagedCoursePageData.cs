using Courses.Domain.Categories;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;

namespace Courses.Application.Features.Management.ManagedCoursePage;

public sealed record ManagedCoursePageData(
    Course Course,
    List<Module> Modules,
    List<Lesson> Lessons,
    Category? Category);
