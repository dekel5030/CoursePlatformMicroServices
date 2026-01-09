namespace Kernel;

public interface ISingleValueObject<TValue>
{
    TValue Value { get; }
}