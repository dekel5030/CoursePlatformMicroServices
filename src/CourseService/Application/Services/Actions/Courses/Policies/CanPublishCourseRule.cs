using Courses.Domain.Courses;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Auth.AuthTypes;

namespace Courses.Application.Services.Actions.Courses.Policies;

internal sealed class CanPublishCourseRule : ICourseActionRule
{
    public IEnumerable<CourseAction> Evaluate(CourseState state, IUserContext userContext)
    {
        Result canPublishResult = CoursePolicies.CanPublish(state.Status, state.LessonCount);
        if (canPublishResult.IsFailure)
        {
            yield break;
        }

        var resourceId = ResourceId.Create(state.CourseId.ToString());
        Guid userId = userContext.Id!.Value;

        bool isOwner = state.InstructorId.Value == userId;
        bool hasPermission = userContext.HasPermission(ActionType.Update, ResourceType.Course, resourceId);

        if (isOwner || hasPermission)
        {
            yield return CourseAction.Publish;
        }
    }
}
