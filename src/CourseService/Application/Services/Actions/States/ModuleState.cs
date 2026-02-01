using Courses.Application.Services.Actions;
using Courses.Domain.Modules.Primitives;

namespace Courses.Application.Services.Actions.States;

public sealed record ModuleState(ModuleId Id) : ILinkEligibilityContext
{
    public Guid ResourceId => Id.Value;
    public Guid? OwnerId => null;
    public object? Status => null;
}
