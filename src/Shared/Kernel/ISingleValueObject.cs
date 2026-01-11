namespace Kernel;

public interface ISingleValueObject<out TValue>
{
    TValue Value { get; }
}
