namespace SharedKernel;

public sealed record Money(decimal Amount, string Currency)
{
    public static Money Zero(string currency = "ILS") => new(0m, currency);
    public Money Multiply(decimal qty) => new(Amount * qty, Currency);
    public static Money operator +(Money a, Money b)
        => a.Currency == b.Currency ? new(a.Amount + b.Amount, a.Currency)
                                    : throw new InvalidOperationException("Currency mismatch");
}
