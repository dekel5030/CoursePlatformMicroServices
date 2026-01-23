using Courses.Application.Actions.Abstract;
using Courses.Application.Actions.CourseCollection.Policies;
using Kernel.Auth.Abstractions;

namespace Courses.Application.Actions.CourseCollection;

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
