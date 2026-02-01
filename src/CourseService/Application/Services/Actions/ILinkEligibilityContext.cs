namespace Courses.Application.Services.Actions;

/// <summary>
/// Minimal context for link eligibility: resource id, optional owner, and optional status for domain rules.
/// State records and composite link contexts implement this so policies can depend on a single interface where possible.
/// </summary>
public interface ILinkEligibilityContext
{
    Guid ResourceId { get; }
    Guid? OwnerId { get; }
    object? Status { get; }
}
