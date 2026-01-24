using Courses.Application.Services.Actions.Abstractions;
using Courses.Application.Services.Actions.CourseCollection.Policies;
using Kernel.Auth.Abstractions;

namespace Courses.Application.Services.Actions.CourseCollection;

public interface ICourseCollectionActionProvider : IActionProvider<CourseCollectionAction, Empty>;

public sealed class CourseCollectionActionProvider
    : ActionProviderBase<CourseCollectionAction, Empty, ICourseCollectionActionRule>
{
    public CourseCollectionActionProvider(
        IUserContext userContext,
        IEnumerable<ICourseCollectionActionRule> rules) : base(userContext, rules)
    {
    }
}
