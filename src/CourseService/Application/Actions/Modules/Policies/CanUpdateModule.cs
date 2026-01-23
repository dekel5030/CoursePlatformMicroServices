using System;
using System.Collections.Generic;
using System.Text;
using Kernel.Auth.Abstractions;

namespace Courses.Application.Actions.Modules.Policies;

internal sealed class CanUpdateModule : IModuleActionRule
{
    public IEnumerable<ModuleAction> Evaluate(ModuleState state, IUserContext userContext)
    {
        yield return ModuleAction.Update;
        yield return ModuleAction.CreateLesson;
    }
}
