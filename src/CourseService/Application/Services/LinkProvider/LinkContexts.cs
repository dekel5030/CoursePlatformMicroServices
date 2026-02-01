using Courses.Application.Services.Actions;
using Courses.Application.Services.Actions.States;
using Courses.Application.Shared.Dtos;

namespace Courses.Application.Services.LinkProvider;

internal sealed record ModuleLinkContext(CourseState CourseState, ModuleState ModuleState) : ILinkEligibilityContext
{
    public Guid ResourceId => ModuleState.Id.Value;
    public Guid? OwnerId => CourseState.InstructorId.Value;
    object? ILinkEligibilityContext.Status => CourseState.Status;
}

internal sealed record LessonLinkContext(
    CourseState CourseState,
    ModuleState ModuleState,
    LessonState LessonState,
    EnrollmentState? EnrollmentState) : ILinkEligibilityContext
{
    public Guid ResourceId => LessonState.Id.Value;
    public Guid? OwnerId => CourseState.InstructorId.Value;
    object? ILinkEligibilityContext.Status => LessonState.LessonAccess;
}

internal sealed record CourseCollectionContext(PagedQueryDto Query, int TotalCount) : ILinkEligibilityContext
{
    public Guid ResourceId => Guid.Empty;
    public Guid? OwnerId => null;
    public object? Status => null;
}
