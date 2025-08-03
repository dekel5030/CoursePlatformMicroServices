namespace Common.Rollback;

public interface IRollbackManager
{
    void Add(Func<Task> action, Action<Exception>? onException = null);
    Task ExecuteAllAsync(bool continueOnFailure = true);
}