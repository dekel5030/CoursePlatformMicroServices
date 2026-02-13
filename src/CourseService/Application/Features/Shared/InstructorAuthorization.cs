using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;

namespace Courses.Application.Features.Shared;

public static class InstructorAuthorization
{
    public static Result<UserId> EnsureInstructorAuthorized(
        IUserContext userContext,
        UserId courseInstructorId)
    {
        if (userContext.Id is null || !userContext.IsAuthenticated)
        {
            return Result.Failure<UserId>(CourseErrors.Unauthorized);
        }

        var currentUserId = new UserId(userContext.Id.Value);
        if (courseInstructorId != currentUserId)
        {
            return Result.Failure<UserId>(CourseErrors.Unauthorized);
        }

        return Result.Success(courseInstructorId);
    }
}
