namespace Courses.Application.Actions.Abstractions;

public interface IActionProvider<out TAction, TState>
{
    IReadOnlyCollection<TAction> GetAllowedActions(TState state);
}
