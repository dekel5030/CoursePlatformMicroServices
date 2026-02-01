using Courses.Domain.Courses.Primitives;

namespace Courses.Application.Services.Actions.States;

public record CourseState(
    CourseId Id,
    UserId InstructorId,
    CourseStatus Status
) : ILinkEligibilityContext
{
    public Guid ResourceId => Id.Value;
    public Guid? OwnerId => InstructorId.Value;
    object? ILinkEligibilityContext.Status => Status;
}
