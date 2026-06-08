namespace Antro.Domain;

public readonly record struct Currency
{
    public static Currency Rub { get; } = new("RUB");

    public Currency(string code)
    {
        Code = DomainGuard.RequiredText(code, nameof(code)).ToUpperInvariant();
    }

    public string Code { get; }

    public override string ToString() => Code;
}

public enum AmountPeriod
{
    OneTime = 0,
    Monthly = 1,
    Yearly = 2,
    NotApplicable = 3
}

public readonly record struct MoneyAmount
{
    public MoneyAmount(decimal value, Currency currency, AmountPeriod period)
    {
        if (value < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Money amount cannot be negative.");
        }

        Value = value;
        Currency = currency;
        Period = period;
    }

    public decimal Value { get; }

    public Currency Currency { get; }

    public AmountPeriod Period { get; }

    public override string ToString() => $"{Value:0.##} {Currency} / {Period}";
}
