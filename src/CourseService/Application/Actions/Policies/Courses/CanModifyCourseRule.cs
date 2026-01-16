using Courses.Application.Actions.Primitives;
using Courses.Domain.Courses;
using Kernel.Auth.Abstractions;
using Kernel.Auth.AuthTypes;

namespace Courses.Application.Actions.Policies.Courses;

internal sealed class CanModifyCourseRule : ICourseActionRule
{
    public IEnumerable<CourseAction> Evaluate(Course course, IUserContext userContext)
    {
        if (!course.CanBeModified.IsSuccess)
        {
            yield break;
        }

        var resourceId = ResourceId.Create(course.Id.ToString());
        Guid userId = userContext.Id!.Value;

        bool isOwner = course.InstructorId?.Value == userId;
        bool hasPermission = userContext.HasPermission(ActionType.Update, ResourceType.Course, resourceId);

        if (isOwner || hasPermission)
        {
            yield return CourseAction.Update;
            yield return CourseAction.CreateLesson;
        }
    }
}
