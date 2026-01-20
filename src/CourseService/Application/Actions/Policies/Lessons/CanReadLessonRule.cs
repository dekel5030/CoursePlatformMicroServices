using Courses.Application.Actions.Primitives;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Kernel.Auth.Abstractions;
using Kernel.Auth.AuthTypes;

namespace Courses.Application.Actions.Policies.Lessons;

internal sealed class CanReadLessonRule : ILessonActionRule
{
    public IEnumerable<LessonAction> Evaluate(Course course, Lesson lesson, IUserContext userContext)
    {
        var resourceId = ResourceId.Create(course.Id.ToString());
        Guid userId = userContext.Id!.Value;

        bool isOwner = course.InstructorId.Value == userId;
        bool hasPermission = userContext.HasPermission(ActionType.Read, ResourceType.Course, resourceId);

        if (isOwner || hasPermission)
        {
            yield return LessonAction.Read;
        }
    }
}
