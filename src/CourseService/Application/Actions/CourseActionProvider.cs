using Courses.Application.Actions.Abstract;
using Courses.Application.Actions.Policies.CourseCollection;
using Courses.Application.Actions.Policies.Courses;
using Courses.Application.Actions.Policies.Lessons;
using Courses.Application.Actions.Primitives;
using Kernel.Auth.Abstractions;

namespace Courses.Application.Actions;

public sealed class CourseActionProvider : ICourseActionProvider
{
    private readonly IUserContext _userContext;
    private readonly IEnumerable<ICourseActionRule> _courseRules;
    private readonly IEnumerable<ICourseCollectionActionRule> _collectionRules;
    private readonly IEnumerable<ILessonActionRule> _lessonRules;

    public CourseActionProvider(
        IUserContext userContext,
        IEnumerable<ICourseActionRule> courseRules,
        IEnumerable<ICourseCollectionActionRule> collectionRules,
        IEnumerable<ILessonActionRule> lessonRules)
    {
        _userContext = userContext;
        _courseRules = courseRules;
        _collectionRules = collectionRules;
        _lessonRules = lessonRules;
    }

    public IReadOnlyCollection<CourseAction> GetAllowedActions(CoursePolicyContext context)
    {
        if (_userContext.Id is null)
        {
            return Array.Empty<CourseAction>();
        }

        var actions = new HashSet<CourseAction>();

        foreach (ICourseActionRule rule in _courseRules)
        {
            foreach (CourseAction action in rule.Evaluate(context, _userContext))
            {
                actions.Add(action);
            }
        }

        return actions;
    }

    public IReadOnlyCollection<LessonAction> GetAllowedActions(
        CoursePolicyContext courseContext,
        LessonPolicyContext lessonContext)
    {
        if (_userContext.Id is null)
        {
            return Array.Empty<LessonAction>();
        }

        var actions = new HashSet<LessonAction>();

        foreach (ILessonActionRule rule in _lessonRules)
        {
            foreach (LessonAction action in rule.Evaluate(courseContext, lessonContext, _userContext))
            {
                actions.Add(action);
            }
        }

        return actions;
    }

    public IReadOnlyCollection<CourseCollectionAction> GetAllowedCollectionActions()
    {
        if (_userContext.Id is null)
        {
            return Array.Empty<CourseCollectionAction>();
        }

        var actions = new HashSet<CourseCollectionAction>();

        foreach (ICourseCollectionActionRule rule in _collectionRules)
        {
            foreach (CourseCollectionAction action in rule.Evaluate(_userContext))
            {
                actions.Add(action);
            }
        }

        return actions;
    }
}
