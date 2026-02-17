using Kernel;

namespace Courses.Domain.Courses.Errors;

public static class CourseErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Course.NotFound",
        "Course Not Found.");

    public static readonly Error AlreadyPublished = Error.Conflict(
        "Course.AlreadyPublished",
        "Course Already Published.");

    public static readonly Error CourseWithoutLessons = Error.Validation(
        "Course.WithoutLessons",
        "Course Could Not Be Published Without Lessons.");

    public static readonly Error LessonAlreadyExists = Error.Validation(
        "Course.LessonAlreadyExists",
        "Course Lesson Already Exists.");

    public static readonly Error InvalidPrice = Error.Validation(
        "Course.InvalidPrice",
        "Course price is invalid.");

    public static readonly Error CourseNotPublished = Error.Validation(
        "Course.NotPublished",
        "Course isnt published.");

    public static readonly Error CannotModifyDeleted = Error.Validation(
        "Course.CannotModifyDeleted",
        "Cannot modify a deleted course.");

    public static readonly Error CannotChangePriceWhenPublished = Error.Validation(
        "Course.CannotChangePriceWhenPublished",
        "Cannot change the price of a published course.");

    public static readonly Error Unauthorized = Error.Unauthorized(
        "Course.Unauthorized",
        "You are not authorized to perform this action.");
}
