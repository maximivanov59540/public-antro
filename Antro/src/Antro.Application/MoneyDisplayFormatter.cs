using System.Globalization;
using Antro.Domain;

namespace Antro.Application;

internal static class MoneyDisplayFormatter
{
    public static string FormatAmount(MoneyAmount amount) =>
        $"{FormatMoneyValue(amount.Value)} {FormatCurrency(amount.Currency)}{FormatPeriodSuffix(amount.Period)}";

    public static string FormatRange(MoneyAmount minimumAmount, MoneyAmount maximumAmount)
    {
        EnsureComparable(minimumAmount, maximumAmount);
        return $"{FormatMoneyValue(minimumAmount.Value)}\u2013{FormatMoneyValue(maximumAmount.Value)} {FormatCurrency(minimumAmount.Currency)}{FormatPeriodSuffix(minimumAmount.Period)}";
    }

    public static string FormatUpTo(MoneyAmount maximumAmount) =>
        $"\u0434\u043e {FormatAmount(maximumAmount)}";

    public static string FormatPercentage(decimal percentage, MoneyAmount? capAmount)
    {
        var text = $"{FormatDecimal(percentage)}%";
        return capAmount is null ? text : $"{text} (\u0434\u043e {FormatAmount(capAmount.Value)})";
    }

    private static void EnsureComparable(MoneyAmount minimumAmount, MoneyAmount maximumAmount)
    {
        if (minimumAmount.Currency != maximumAmount.Currency)
        {
            throw new ArgumentException("Money amounts must use the same currency.");
        }

        if (minimumAmount.Period != maximumAmount.Period)
        {
            throw new ArgumentException("Money amounts must use the same period.");
        }
    }

    private static string FormatCurrency(Currency currency) =>
        currency == Currency.Rub ? "\u20BD" : currency.Code;

    private static string FormatPeriodSuffix(AmountPeriod period) =>
        period switch
        {
            AmountPeriod.Monthly => "/\u043c\u0435\u0441",
            AmountPeriod.Yearly => "/\u0433\u043e\u0434",
            _ => string.Empty
        };

    private static string FormatMoneyValue(decimal value)
    {
        var wholePart = decimal.Truncate(value);
        var wholeText = wholePart
            .ToString("N0", CultureInfo.InvariantCulture)
            .Replace(",", " ", StringComparison.Ordinal);

        var fractionalPart = value - wholePart;
        if (fractionalPart == 0m)
        {
            return wholeText;
        }

        var normalized = value.ToString("0.##", CultureInfo.InvariantCulture);
        var parts = normalized.Split('.');
        return $"{wholeText},{parts[1]}";
    }

    private static string FormatDecimal(decimal value) =>
        value % 1m == 0m
            ? value.ToString("0", CultureInfo.InvariantCulture)
            : value.ToString("0.##", CultureInfo.InvariantCulture).Replace(".", ",", StringComparison.Ordinal);
}
