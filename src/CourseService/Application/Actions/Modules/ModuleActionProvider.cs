using Courses.Application.Actions.Abstractions;
using Courses.Application.Actions.Modules.Policies;
using Kernel.Auth.Abstractions;

namespace Courses.Application.Actions.Modules;

public sealed class ModuleActionProvider
    : ActionProviderBase<ModuleAction, ModuleState, IModuleActionRule>, IModuleActionProvider
{
    public ModuleActionProvider(IUserContext userContext, IEnumerable<IModuleActionRule> rules) : base(userContext, rules)
    {
    }
}
