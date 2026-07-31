namespace SplitSpace.Finances.Domain.Models.ValueObjects;

public readonly record struct Money(decimal Amount)
{
    public Money Negate() => new(-Amount);

    public Money Add(Money other)
    {
        return new Money(Amount + other.Amount);
    }

    public Money Subtract(Money other)
    {
        return new Money(Amount - other.Amount);
    }
    
    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);
    public static Money operator -(Money value) => value.Negate();
}
