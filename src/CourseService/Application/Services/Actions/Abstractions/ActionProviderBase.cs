using Kernel.Auth.Abstractions;

namespace Courses.Application.Services.Actions.Abstractions;

public abstract class ActionProviderBase<TAction, TState, TRule> : IActionProvider<TAction, TState>
    where TRule : IActionRule<TAction, TState>
{
    private readonly IUserContext _userContext;
    private readonly IEnumerable<TRule> _rules;

    protected ActionProviderBase(IUserContext userContext, IEnumerable<TRule> rules)
    {
        _userContext = userContext;
        _rules = rules;
    }

    public virtual IReadOnlyCollection<TAction> GetAllowedActions(TState state)
    {
        if (_userContext.Id is null)
        {
            return Array.Empty<TAction>();
        }

        var actions = new HashSet<TAction>();

        foreach (TRule rule in _rules)
        {
            foreach (TAction action in rule.Evaluate(state, _userContext))
            {
                actions.Add(action);
            }
        }

        return actions;
    }
}
