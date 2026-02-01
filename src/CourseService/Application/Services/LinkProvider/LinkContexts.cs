using Courses.Application.Services.Actions.States;
using Courses.Application.Shared.Dtos;

namespace Courses.Application.Services.LinkProvider;

internal sealed record ModuleLinkContext(CourseState CourseState, ModuleState ModuleState);

internal sealed record LessonLinkContext(
    CourseState CourseState,
    ModuleState ModuleState,
    LessonState LessonState,
    EnrollmentState? EnrollmentState);

internal sealed record CourseCollectionContext(PagedQueryDto Query, int TotalCount);
