namespace Courses.Application.Services.Actions.Abstractions;

public interface IActionProvider<out TAction, TState>
{
    IReadOnlyCollection<TAction> GetAllowedActions(TState state);
}
