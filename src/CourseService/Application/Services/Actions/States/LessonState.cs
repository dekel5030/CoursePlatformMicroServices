using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Services.Actions.States;

public sealed record LessonState(LessonId Id, LessonAccess LessonAccess) : ILinkEligibilityContext
{
    public Guid ResourceId => Id.Value;
    public Guid? OwnerId => null;
    public object? Status => LessonAccess;
}
