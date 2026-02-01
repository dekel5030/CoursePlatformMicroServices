using Courses.Application.Services.Actions.States;
using Courses.Application.Services.LinkProvider;
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

        Result canPublishResult = CoursePolicies.CanPublish(state.Status);
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

    /// <summary>
    /// Consolidated entry point for course actions. Uses ILinkEligibilityContext (ResourceId, OwnerId, Status).
    /// </summary>
    public bool Can(CourseAction action, ILinkEligibilityContext context)
    {
        CourseStatus status = context.Status as CourseStatus? ?? default;
        var courseState = new CourseState(
            new CourseId(context.ResourceId),
            new UserId(context.OwnerId ?? Guid.Empty),
            status);

        return action switch
        {
            CourseAction.Read => CanReadCourse(courseState),
            CourseAction.Update => CanEditCourseContent(courseState),
            CourseAction.Delete => CanDeleteCourse(courseState),
            CourseAction.GenerateImageUploadUrl => CanEditCourseContent(courseState),
            CourseAction.CreateModule => CanEditCourseContent(courseState),
            _ => false
        };
    }

    /// <summary>
    /// Consolidated entry point for lesson actions. Requires LessonLinkContext for course/lesson/enrollment.
    /// </summary>
    public bool Can(LessonAction action, ILinkEligibilityContext context)
    {
        if (context is not LessonLinkContext lessonContext)
        {
            return false;
        }

        return action switch
        {
            LessonAction.Read => CanReadLesson(lessonContext.CourseState, lessonContext.LessonState, lessonContext.EnrollmentState),
            LessonAction.Update => CanEditLesson(lessonContext.CourseState),
            LessonAction.Delete => CanEditLesson(lessonContext.CourseState),
            LessonAction.UploadVideoUrl => CanEditLesson(lessonContext.CourseState),
            LessonAction.AiGenerate => CanEditLesson(lessonContext.CourseState),
            _ => false
        };
    }

    /// <summary>
    /// Consolidated entry point for module actions. Requires ModuleLinkContext for course scope.
    /// </summary>
    public bool Can(ModuleAction action, ILinkEligibilityContext context)
    {
        if (context is not ModuleLinkContext moduleContext)
        {
            return false;
        }

        return action switch
        {
            ModuleAction.CreateLesson => CanEditModule(moduleContext.CourseState),
            _ => false
        };
    }

    /// <summary>
    /// Consolidated entry point for course collection actions.
    /// </summary>
    public bool Can(CourseCollectionAction action, ILinkEligibilityContext context)
    {
        if (action is CourseCollectionAction.NextPage or CourseCollectionAction.PreviousPage
            && context is CourseCollectionContext collectionContext)
        {
            int page = collectionContext.Query.Page ?? 1;
            int pageSize = collectionContext.Query.PageSize ?? 10;
            return action switch
            {
                CourseCollectionAction.NextPage => page * pageSize < collectionContext.TotalCount,
                CourseCollectionAction.PreviousPage => page > 1,
                _ => false
            };
        }

        return action switch
        {
            CourseCollectionAction.Self => true,
            CourseCollectionAction.Create => CanCreateCourse(),
            _ => false
        };
    }
}
