namespace Courses.Application.Actions.Abstract;

public interface IActionProvider<out TAction, TState>
{
    IReadOnlyCollection<TAction> GetAllowedActions(TState state);
}
