using Antro.Domain;

namespace Antro.Content.Moscow2026;

public sealed record Moscow2026Thresholds
{
    public Moscow2026Thresholds(
        MoneyAmount livingMinimumPerCapita,
        MoneyAmount livingMinimumWorkingAge,
        MoneyAmount livingMinimumChild,
        MoneyAmount unifiedBenefitIncomeThreshold,
        MoneyAmount annualFamilyPaymentThreshold,
        MoneyAmount matCapitalMonthlyPaymentThreshold)
    {
        LivingMinimumPerCapita = RequireRub(livingMinimumPerCapita, nameof(livingMinimumPerCapita));
        LivingMinimumWorkingAge = RequireRub(livingMinimumWorkingAge, nameof(livingMinimumWorkingAge));
        LivingMinimumChild = RequireRub(livingMinimumChild, nameof(livingMinimumChild));
        UnifiedBenefitIncomeThreshold = RequireRub(unifiedBenefitIncomeThreshold, nameof(unifiedBenefitIncomeThreshold));
        AnnualFamilyPaymentThreshold = RequireRub(annualFamilyPaymentThreshold, nameof(annualFamilyPaymentThreshold));
        MatCapitalMonthlyPaymentThreshold = RequireRub(matCapitalMonthlyPaymentThreshold, nameof(matCapitalMonthlyPaymentThreshold));
    }

    public MoneyAmount LivingMinimumPerCapita { get; }

    public MoneyAmount LivingMinimumWorkingAge { get; }

    public MoneyAmount LivingMinimumChild { get; }

    public MoneyAmount UnifiedBenefitIncomeThreshold { get; }

    public MoneyAmount AnnualFamilyPaymentThreshold { get; }

    public MoneyAmount MatCapitalMonthlyPaymentThreshold { get; }

    private static MoneyAmount RequireRub(MoneyAmount amount, string paramName)
    {
        if (amount.Currency != Currency.Rub)
        {
            throw new ArgumentException("Moscow 2026 thresholds must be expressed in RUB.", paramName);
        }

        return amount;
    }
}
