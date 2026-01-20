using Courses.Application.Actions.Primitives;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Kernel.Auth.Abstractions;

namespace Courses.Application.Actions.Policies.Lessons;

public interface ILessonActionRule
{
    IEnumerable<LessonAction> Evaluate(
        CoursePolicyContext courseContext,
        LessonPolicyContext lessonContext,
        IUserContext userContext);
}
