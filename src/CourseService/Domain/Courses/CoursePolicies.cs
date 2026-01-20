using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Domain.Courses;

public static class CoursePolicies
{
    public static Result CanModify(CourseStatus status)
    {
        return status != CourseStatus.Deleted
            ? Result.Success()
            : Result.Failure(CourseErrors.CannotModifyDeleted);
    }

    public static Result CanDelete(CourseStatus status)
    {
        return status == CourseStatus.Deleted
            ? Result.Success()
            : Result.Failure(CourseErrors.NotFound);
    }

    public static Result CanEnroll(CourseStatus status)
    {
        if (status != CourseStatus.Published)
        {
            return Result.Failure(CourseErrors.CourseNotPublished);
        }

        return Result.Success();
    }

    public static Result CanPublish(CourseStatus status, int lessonCount)
    {
        if (status == CourseStatus.Deleted)
        {
            return Result.Failure(CourseErrors.NotFound);
        }

        if (status == CourseStatus.Published)
        {
            return Result.Failure(CourseErrors.AlreadyPublished);
        }

        if (lessonCount == 0)
        {
            return Result.Failure(CourseErrors.CourseWithoutLessons);
        }

        return Result.Success();
    }
}
