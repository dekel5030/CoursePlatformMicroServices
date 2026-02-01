namespace Courses.Application.Services.LinkProvider.Abstractions;

public interface ILinkEligibilityContext
{
    Guid ResourceId { get; }
    Guid? OwnerId { get; }
    object? Status { get; }
}
