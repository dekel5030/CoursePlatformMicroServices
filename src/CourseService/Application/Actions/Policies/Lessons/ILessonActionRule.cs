using Courses.Application.Actions.Primitives;
using Kernel.Auth.Abstractions;

namespace Courses.Application.Actions.Policies.Lessons;

public interface ILessonActionRule
{
    IEnumerable<LessonAction> Evaluate(
        CoursePolicyContext courseContext,
        LessonPolicyContext lessonContext,
        IUserContext userContext);
}
