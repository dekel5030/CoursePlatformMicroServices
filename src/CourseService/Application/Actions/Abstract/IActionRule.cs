using Kernel.Auth.Abstractions;

namespace Courses.Application.Actions.Abstract;

public interface IActionRule<out TAction, in TState>
{
    IEnumerable<TAction> Evaluate(TState state, IUserContext userContext);
}
