using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Domain.Courses;

public static class CoursePolicies
{
    public static Result CanModify(bool isDeleted)
    {
        return !isDeleted
            ? Result.Success()
            : Result.Failure(CourseErrors.CannotModifyDeleted);
    }

    public static Result CanDelete(bool isDeleted)
    {
        return !isDeleted
            ? Result.Success()
            : Result.Failure(CourseErrors.NotFound);
    }

    public static Result CanEnroll(bool isDeleted, CourseStatus status)
    {
        if (isDeleted)
        {
            return Result.Failure(CourseErrors.NotFound);
        }

        if (status != CourseStatus.Published)
        {
            return Result.Failure(CourseErrors.CourseNotPublished);
        }

        return Result.Success();
    }

    public static Result CanPublish(bool isDeleted, CourseStatus status, int lessonCount)
    {
        if (isDeleted)
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
