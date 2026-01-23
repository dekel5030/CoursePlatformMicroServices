using Courses.Application.Actions.Abstractions;
using Courses.Application.Actions.Courses.Policies;
using Kernel.Auth.Abstractions;

namespace Courses.Application.Actions.Courses;

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
