using Kernel;

namespace Courses.Domain.Module.Errors;

public static class ModuleErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Module.NotFound", "The specified module was not found.");
}
