using Kernel;

namespace Courses.Domain.Categories.Errors;

public static class CategoryErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Category.NotFound",
        "The specified category was not found.");
}
