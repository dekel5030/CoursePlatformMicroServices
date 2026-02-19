using Kernel;

namespace Courses.Domain.MediaPackages.Errors;

public static class MediaPackageErrors
{
    public static readonly Error CannotChangePublishedPackage = Error.Validation(
        "MediaPackage.CannotChnagePublished",
        "Cannot change a published media package.");

    public static readonly Error NoAssets = Error.Validation(
        "MediaPackage.NoAssets",
        "Cannot publish a package without assets.");
}
