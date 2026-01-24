using Courses.Application.Services.Actions.Abstractions;
using Kernel.Auth.Abstractions;

namespace Courses.Application.Services.Actions.Lessons;

public sealed class LessonActionProvider
    : ActionProviderBase<LessonAction, LessonState, ILessonActionRule>, ILessonActionProvider
{
    public LessonActionProvider(
        IUserContext userContext,
        IEnumerable<ILessonActionRule> rules) : base(userContext, rules)
    {
    }
}
