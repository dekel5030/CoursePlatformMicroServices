namespace SharedKernel;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
