using Kernel.Auth.Abstractions;

namespace Courses.Application.Services.Actions.Modules.Policies;

internal sealed class CanUpdateModule : IModuleActionRule
{
    public IEnumerable<ModuleAction> Evaluate(ModuleState state, IUserContext userContext)
    {
        yield return ModuleAction.Update;
        yield return ModuleAction.CreateLesson;
    }
}
