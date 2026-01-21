using Courses.Application.Actions.Primitives;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel.Auth.Abstractions;
using Kernel.Auth.AuthTypes;

namespace Courses.Application.Actions.Policies.Lessons;

internal sealed class CanReadLessonRule : ILessonActionRule
{
    public IEnumerable<LessonAction> Evaluate(
        CoursePolicyContext courseContext,
        LessonPolicyContext lessonContext,
        IUserContext userContext)
    {
        var resourceId = ResourceId.Create(courseContext.CourseId.ToString());
        Guid userId = userContext.Id!.Value;

        bool isOwner = courseContext.InstructorId.Value == userId;
        bool hasPermission = userContext.HasPermission(ActionType.Read, ResourceType.Course, resourceId);

        if (isOwner || hasPermission)
        {
            yield return LessonAction.Read;
            yield break;
        }

        if (courseContext.Status != CourseStatus.Published
            || lessonContext.Status != LessonStatus.Published)
        {
            yield break;
        }

        if (lessonContext.Access == LessonAccess.Public)
        {
            yield return LessonAction.Read;
            yield break;
        }
    }
}
