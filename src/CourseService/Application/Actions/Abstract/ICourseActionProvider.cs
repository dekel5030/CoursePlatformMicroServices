using Courses.Application.Actions.Primitives;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;

namespace Courses.Application.Actions.Abstract;

public interface ICourseActionProvider
{
    IReadOnlyCollection<CourseAction> GetAllowedActions(CoursePolicyContext context);
    IReadOnlyCollection<LessonAction> GetAllowedActions(
        CoursePolicyContext courseContext,
        LessonPolicyContext lessonContext);
    IReadOnlyCollection<CourseCollectionAction> GetAllowedCollectionActions();
}
