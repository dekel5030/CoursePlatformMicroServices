using Courses.Application.Actions.Primitives;
using Courses.Domain.Courses;
using Kernel.Auth.Abstractions;
using Kernel.Auth.AuthTypes;

namespace Courses.Application.Actions.Policies.Courses;

internal sealed class CanReadCourseRule : ICourseActionRule
{
    public IEnumerable<CourseAction> Evaluate(Course course, IUserContext userContext)
    {
        var resourceId = ResourceId.Create(course.Id.Value.ToString());
        Guid userId = userContext.Id!.Value;

        bool isOwner = course.InstructorId?.Value == userId;
        bool hasPermission = userContext.HasPermission(ActionType.Read, ResourceType.Course, resourceId);

        if (isOwner || hasPermission)
        {
            yield return CourseAction.Read;
        }
    }
}
