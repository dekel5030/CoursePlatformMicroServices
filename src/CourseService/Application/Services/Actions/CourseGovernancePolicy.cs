using Courses.Application.Services.Actions.States;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Auth.AuthTypes;

namespace Courses.Application.Services.Actions;

public class CourseGovernancePolicy
{
    private readonly IUserContext _userContext;

    public CourseGovernancePolicy(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public bool CanEditCourseContent(CourseState state)
    {
        if (_userContext.Id is null)
        {
            return false;
        }

        Result canModifyResult = CoursePolicies.CanModify(state.Status);
        if (canModifyResult.IsFailure)
        {
            return false;
        }

        Guid userId = _userContext.Id.Value;
        bool isOwner = state.InstructorId.Value == userId;
        bool hasPermission = _userContext.HasPermission(ActionType.Update, ResourceType.Course, ResourceId.Create(state.Id.ToString()));

        return isOwner || hasPermission;
    }

    public bool CanDeleteCourse(CourseState state)
    {
        if (_userContext.Id is null)
        {
            return false;
        }

        Result canDeleteResult = CoursePolicies.CanDelete(state.Status);
        if (canDeleteResult.IsFailure)
        {
            return false;
        }

        Guid userId = _userContext.Id.Value;
        bool isOwner = state.InstructorId.Value == userId;
        bool hasPermission = _userContext.HasPermission(ActionType.Delete, ResourceType.Course, ResourceId.Create(state.Id.ToString()));

        return isOwner || hasPermission;
    }

    public bool CanPublishCourse(CourseState state)
    {
        if (_userContext.Id is null)
        {
            return false;
        }

        Result canPublishResult = CoursePolicies.CanPublish(state.Status, state.LessonCount);
        if (canPublishResult.IsFailure)
        {
            return false;
        }

        Guid userId = _userContext.Id.Value;
        bool isOwner = state.InstructorId.Value == userId;
        bool hasPermission = _userContext.HasPermission(ActionType.Update, ResourceType.Course, ResourceId.Create(state.Id.ToString()));

        return isOwner || hasPermission;
    }

    public bool CanReadCourse(CourseState state)
    {
        if (state.Status == CourseStatus.Published)
        {
            return true;
        }

        if (_userContext.Id is null)
        {
            return false;
        }

        Guid userId = _userContext.Id.Value;
        bool isOwner = state.InstructorId.Value == userId;
        bool hasPermission = _userContext.HasPermission(ActionType.Read, ResourceType.Course, ResourceId.Create(state.Id.Value.ToString()));

        return isOwner || hasPermission;
    }

    public bool CanCreateCourse()
    {
        if (_userContext.Id is null)
        {
            return false;
        }
        return _userContext.HasPermission(ActionType.Create, ResourceType.Course, ResourceId.Wildcard);
    }

    public bool CanReadLesson(
        CourseState courseState,
        LessonState lessonState,
        EnrollmentState? enrollmentState = null)
    {
        if (CanEditCourseContent(courseState))
        {
            return true;
        }

        if (courseState.Status != CourseStatus.Published)
        {
            return false;
        }

        if (lessonState.LessonAccess == LessonAccess.Public)
        {
            return true;
        }

        return enrollmentState?.HasEnrollment ?? false;
    }

    public bool CanEditLesson(CourseState courseState)
    {
        return CanEditCourseContent(courseState);
    }

    public bool CanEditModule(CourseState courseState)
    {
        return CanEditCourseContent(courseState);
    }
}
