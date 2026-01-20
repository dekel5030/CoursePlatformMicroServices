using Courses.Application.Actions.Primitives;
using Courses.Domain.Courses;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Auth.AuthTypes;

namespace Courses.Application.Actions.Policies.Courses;

internal sealed class CanModifyCourseRule : ICourseActionRule
{
    public IEnumerable<CourseAction> Evaluate(CoursePolicyContext context, IUserContext userContext)
    {
        Result canModifyResult = CoursePolicies.CanModify(context.IsDeleted);
        if (canModifyResult.IsFailure)
        {
            yield break;
        }

        var resourceId = ResourceId.Create(context.CourseId.ToString());
        Guid userId = userContext.Id!.Value;

        bool isOwner = context.InstructorId.Value == userId;
        bool hasPermission = userContext.HasPermission(ActionType.Update, ResourceType.Course, resourceId);

        if (isOwner || hasPermission)
        {
            yield return CourseAction.Update;
            yield return CourseAction.CreateLesson;
            yield return CourseAction.UploadImageUrl;
        }
    }
}
