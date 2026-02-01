using Courses.Application.Services.LinkProvider;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel.Auth.Abstractions;
using Kernel.Auth.AuthTypes;

namespace Courses.Application.Services.Actions;

internal sealed class CourseGovernancePolicy
{
    private readonly IUserContext _userContext;

    public CourseGovernancePolicy(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public bool CanReadCourse(CourseContext ctx)
    {
        if (ctx.Status == CourseStatus.Published)
        {
            return true;
        }

        if (_userContext.Id is null)
        {
            return false;
        }

        return ctx.InstructorId.Value == _userContext.Id.Value ||
            _userContext.HasPermission(
                ActionType.Read,
                ResourceType.Course,
                ResourceId.Create(ctx.Id.Value.ToString()));
    }

    public bool CanEditCourseContent(CourseContext ctx)
    {
        if (_userContext.Id is null || CoursePolicies.CanModify(ctx.Status).IsFailure)
        {
            return false;
        }

        return ctx.InstructorId.Value == _userContext.Id.Value ||
            _userContext.HasPermission(
                ActionType.Update,
                ResourceType.Course,
                ResourceId.Create(ctx.Id.ToString()));
    }

    public bool CanDeleteCourse(CourseContext ctx)
    {
        if (_userContext.Id is null || CoursePolicies.CanDelete(ctx.Status).IsFailure)
        {
            return false;
        }

        return ctx.InstructorId.Value == _userContext.Id.Value ||
            _userContext.HasPermission(
                ActionType.Delete,
                ResourceType.Course,
                ResourceId.Create(ctx.Id.ToString()));
    }

    public bool CanCreateCourse()
    {
        return _userContext.Id is not null &&
               _userContext.HasPermission(ActionType.Create, ResourceType.Course, ResourceId.Wildcard);
    }

    public bool CanReadLesson(LessonContext ctx)
    {
        if (CanEditCourseContent(ctx.Course))
        {
            return true;
        }

        if (ctx.Course.Status != CourseStatus.Published)
        {
            return false;
        }

        if (ctx.Access == LessonAccess.Public)
        {
            return true;
        }

        return ctx.HasEnrollment;
    }

    public bool CanEditModule(ModuleContext ctx) => CanEditCourseContent(ctx.Course);

    public bool CanEditLesson(LessonContext ctx) => CanEditCourseContent(ctx.Course);

    public static bool CanShowNextPage(CourseCollectionContext ctx)
    {
        int page = ctx.Query.Page ?? 1;
        int pageSize = ctx.Query.PageSize ?? 10;
        return page * pageSize < ctx.TotalCount;
    }

    public static bool CanShowPreviousPage(CourseCollectionContext ctx)
    {
        return (ctx.Query.Page ?? 1) > 1;
    }
}
