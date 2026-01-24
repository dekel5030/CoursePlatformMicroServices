using Kernel.Auth.Abstractions;

namespace Courses.Application.Services.Actions.Lessons.Policies;

internal sealed class CanReadLesson : ILessonActionRule
{
    public IEnumerable<LessonAction> Evaluate(LessonState state, IUserContext userContext)
    {
        yield return LessonAction.Update;
        yield return LessonAction.Delete;
        yield return LessonAction.UploadVideoUrl;
        yield return LessonAction.Read;
    }
}
