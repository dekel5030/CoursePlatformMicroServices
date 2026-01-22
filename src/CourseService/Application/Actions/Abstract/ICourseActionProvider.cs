using Courses.Application.Actions.Primitives;

namespace Courses.Application.Actions.Abstract;

public interface ICourseActionProvider
{
    IReadOnlyCollection<CourseAction> GetAllowedActions(CoursePolicyContext context);
    IReadOnlyCollection<LessonAction> GetAllowedActions(
        CoursePolicyContext courseContext,
        LessonPolicyContext lessonContext);
    IReadOnlyCollection<CourseCollectionAction> GetAllowedCollectionActions();
}
