using Courses.Application.Services.Actions.Abstractions;
using Courses.Application.Services.Actions.Courses.Policies;
using Kernel.Auth.Abstractions;

namespace Courses.Application.Services.Actions.Courses;

public sealed class CourseActionProvider :
    ActionProviderBase<CourseAction, CourseState, ICourseActionRule>, ICourseActionProvider
{
    public CourseActionProvider(
        IUserContext userContext,
        IEnumerable<ICourseActionRule> courseRules)
        : base(userContext, courseRules)
    {
    }
}
