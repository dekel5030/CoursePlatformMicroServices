using Courses.Application.Actions.Primitives;
using Courses.Domain.Courses;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Auth.AuthTypes;

namespace Courses.Application.Actions.Policies.Courses;

internal sealed class CanDeleteCourseRule : ICourseActionRule
{
    public IEnumerable<CourseAction> Evaluate(CoursePolicyContext course, IUserContext userContext)
    {
        Result canDeleteResult = CoursePolicies.CanDelete(course.IsDeleted);

        if (canDeleteResult.IsFailure)
        {
            yield break;
        }

        var resourceId = ResourceId.Create(course.CourseId.ToString());
        Guid userId = userContext.Id!.Value;

        bool isOwner = course.InstructorId.Value == userId;
        bool hasPermission = userContext.HasPermission(ActionType.Delete, ResourceType.Course, resourceId);

        if (isOwner || hasPermission)
        {
            yield return CourseAction.Delete;
        }
    }
}
