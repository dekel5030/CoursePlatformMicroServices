using Courses.Application.Actions.Primitives;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Auth.AuthTypes;

namespace Courses.Application.Actions.Policies.Lessons;

internal sealed class CanDeleteLessonRule : ILessonActionRule
{
    public IEnumerable<LessonAction> Evaluate(
        CoursePolicyContext courseContext,
        LessonPolicyContext lessonContext,
        IUserContext userContext)
    {
        Result canModifyResult = CoursePolicies.CanModify(courseContext.IsDeleted);
        if (canModifyResult.IsFailure)
        {
            yield break;
        }

        var resourceId = ResourceId.Create(courseContext.CourseId.ToString());
        Guid userId = userContext.Id!.Value;

        bool isOwner = courseContext.InstructorId.Value == userId;

        bool hasPermission = userContext.HasPermission(ActionType.Update, ResourceType.Course, resourceId);

        if (isOwner || hasPermission)
        {
            yield return LessonAction.Delete;
        }
    }
}
