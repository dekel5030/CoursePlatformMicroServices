using Courses.Application.Actions.Abstractions;
using Kernel.Auth.Abstractions;

namespace Courses.Application.Actions.Lessons;

public sealed class LessonActionProvider 
    : ActionProviderBase<LessonAction, LessonState, ILessonActionRule>, ILessonActionProvider
{
    public LessonActionProvider(
        IUserContext userContext, 
        IEnumerable<ILessonActionRule> rules) : base(userContext, rules)
    {
    }
}
