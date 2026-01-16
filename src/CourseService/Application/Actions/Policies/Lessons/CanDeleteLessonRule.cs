using Courses.Application.Actions.Primitives;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Kernel.Auth.Abstractions;
using Kernel.Auth.AuthTypes;

namespace Courses.Application.Actions.Policies.Lessons;

internal sealed class CanDeleteLessonRule : ILessonActionRule
{
    public IEnumerable<LessonAction> Evaluate(Course course, Lesson lesson, IUserContext userContext)
    {
        if (!course.CanBeModified.IsSuccess)
        {
            yield break;
        }

        var resourceId = ResourceId.Create(course.Id.Value.ToString());
        Guid userId = userContext.Id!.Value;

        bool isOwner = course.InstructorId?.Value == userId;

        bool hasPermission = userContext.HasPermission(ActionType.Update, ResourceType.Course, resourceId);

        if (isOwner || hasPermission)
        {
            yield return LessonAction.Delete;
        }
    }
}
