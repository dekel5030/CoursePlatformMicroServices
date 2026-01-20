using Courses.Application.Actions.Primitives;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Kernel.Auth.Abstractions;
using Kernel.Auth.AuthTypes;

namespace Courses.Application.Actions.Policies.Courses;

internal sealed class CanReadCourseRule : ICourseActionRule
{
    public IEnumerable<CourseAction> Evaluate(CoursePolicyContext context, IUserContext userContext)
    {
        var resourceId = ResourceId.Create(context.CourseId.Value.ToString());
        Guid userId = userContext.Id!.Value;

        bool isOwner = context.InstructorId.Value == userId;
        bool hasPermission = userContext.HasPermission(ActionType.Read, ResourceType.Course, resourceId);

        if (isOwner || hasPermission)
        {
            yield return CourseAction.Read;
            yield break;
        }

        if (context.Status == CourseStatus.Published)
        {
            yield return CourseAction.Read;
        }
    }
}
