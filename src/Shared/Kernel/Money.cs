namespace Kernel;

public sealed record Money(decimal Amount, string Currency)
{
    public static Money Zero(string currency = "ILS") => new(0m, currency);
    public Money Multiply(decimal qty) => new(Amount * qty, Currency);
#pragma warning disable CA2225 // Operator overloads have named alternates
    public static Money operator +(Money a, Money b)
#pragma warning restore CA2225 // Operator overloads have named alternates
        => a.Currency == b.Currency ? new(a.Amount + b.Amount, a.Currency)
                                    : throw new InvalidOperationException("Currency mismatch");
}