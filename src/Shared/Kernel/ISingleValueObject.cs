namespace Kernel;

public interface ISingleValueObject<TSelf, TValue> : IParsable<TSelf>
    where TSelf : ISingleValueObject<TSelf, TValue>
{
    TValue Value { get; }
}