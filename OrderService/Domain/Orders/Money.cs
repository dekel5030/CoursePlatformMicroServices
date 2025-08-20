namespace Domain.Orders;

public class Money
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    public Money(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be null or empty.", nameof(currency));

        Amount = amount;
        Currency = currency;
    }

    public static Money Zero()
    {
        return new Money(0m, "ILS");
    }

    public override string ToString() => $"{Amount} {Currency}";

    internal Money Multiply(decimal quantity)
    {
        throw new NotImplementedException();
    }
}