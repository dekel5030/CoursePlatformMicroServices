namespace Common.Rollback;

public class StackRollbackManager : IRollbackManager
{
    private readonly Stack<(Func<Task> Action, Action<Exception>? OnException)> _actions = new();

    public void Add(Func<Task> action, Action<Exception>? onException = null)
    {
        _actions.Push((action, onException));
    }

    public async Task ExecuteAllAsync(bool continueOnFailure = true)
    {
        while (_actions.Count > 0)
        {
            var (action, onException) = _actions.Pop();

            try
            {
                await action();
            }
            catch (Exception ex)
            {
                onException?.Invoke(ex);

                if (!continueOnFailure)
                {
                    break;
                }
            }
        }
    }
}