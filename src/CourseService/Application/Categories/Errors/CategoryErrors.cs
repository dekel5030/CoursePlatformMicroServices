using Kernel;

namespace Courses.Application.Categories.Errors;

internal static class CategoryErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Category.NotFound",
        "The specified category was not found.");
}
