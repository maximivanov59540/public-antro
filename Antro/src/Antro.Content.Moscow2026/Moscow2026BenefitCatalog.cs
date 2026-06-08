using Antro.Domain;

namespace Antro.Content.Moscow2026;

public static partial class Moscow2026BenefitCatalog
{
    private static readonly Lazy<IReadOnlyList<Benefit>> AllBenefits = new(BuildBenefits);

    public static RegionCode Region => new("RU-MOW");

    public static CatalogVersion CatalogVersion => new("moscow-2026-mvp");

    public static Moscow2026Thresholds Thresholds { get; } = CreateThresholds();

    public static IReadOnlyList<Benefit> All => AllBenefits.Value;

    public static IReadOnlyList<Benefit> GetBenefits() => All;

    private static readonly Lazy<IReadOnlyList<Benefit>> PublishedBenefits = new(BuildPublishedBenefits);

    /// <summary>
    /// Benefit ids that are kept in <see cref="All"/> (the data layer) but excluded from
    /// <see cref="Published"/> because the clean legal register marks them HOLD / DO NOT SHIP:
    /// they still need a dedicated source-pass before being shown to users.
    /// </summary>
    private static readonly IReadOnlyCollection<string> HoldBenefitIds = new HashSet<string>(StringComparer.Ordinal)
    {
        Moscow2026BenefitIds.KindergartenEnrollment.Value,
        Moscow2026BenefitIds.FederalYoungFamilyHousingProgram.Value
    };

    /// <summary>
    /// The user-facing catalog: <see cref="All"/> minus the HOLD / DO NOT SHIP cards.
    /// UI surfaces should bind to this rather than to <see cref="All"/>.
    /// </summary>
    public static IReadOnlyList<Benefit> Published => PublishedBenefits.Value;

    private static IReadOnlyList<Benefit> BuildPublishedBenefits() =>
        All.Where(benefit => !HoldBenefitIds.Contains(benefit.Id.Value)).ToArray();

    // Prozhitochny minimum (PM) for Moscow, 2026 — Postanovlenie Pravitelstva Moskvy ot 11.11.2025 No. 2665-PP.
    // Source: clean legal register (MOS-PM), checked 05.06.2026.
    private static Moscow2026Thresholds CreateThresholds() =>
        new(
            livingMinimumPerCapita: new MoneyAmount(25_342m, Currency.Rub, AmountPeriod.Monthly),
            livingMinimumWorkingAge: new MoneyAmount(28_940m, Currency.Rub, AmountPeriod.Monthly),
            livingMinimumChild: CreateLivingMinimumChildAmount(),
            // Единое пособие: среднедушевой доход ниже 1 ПМ на душу населения.
            unifiedBenefitIncomeThreshold: new MoneyAmount(25_342m, Currency.Rub, AmountPeriod.Monthly),
            // Ежегодная семейная выплата: среднедушевой доход не выше 1,5 ПМ трудоспособного населения (1,5 × 28 940).
            annualFamilyPaymentThreshold: new MoneyAmount(43_410m, Currency.Rub, AmountPeriod.Monthly),
            // Выплата из маткапитала: среднедушевой доход не более 2 ПМ на душу населения (2 × 25 342).
            matCapitalMonthlyPaymentThreshold: new MoneyAmount(50_684m, Currency.Rub, AmountPeriod.Monthly));

    private static MoneyAmount CreateLivingMinimumChildAmount() =>
        new(21_903m, Currency.Rub, AmountPeriod.Monthly);

    private static IReadOnlyList<Benefit> BuildBenefits()
    {
        Benefit?[] benefits =
        [
            PregnancyMonthlyBenefit,
            MaternityLeaveBenefit,
            MoscowEarlyPregnancyRegistrationPayment,
            PregnancyMilkKitchen,
            PregnancyDismissalProtection,
            PregnancyLightWorkRight,
            PregnancyPartTimeRight,
            MoscowNewbornGiftSet,
            FederalBirthGrant,
            MoscowBirthPayment,
            MoscowYoungFamilyPayment,
            MaternityCapital,
            ChildCareMonthlyAllowance,
            UnifiedChildBenefit,
            MatCapitalMonthlyPayment,
            ChildCareLeaveRight,
            ParentDismissalProtection,
            FeedingBreaksRight,
            ChildMilkKitchen,
            KindergartenEnrollment,
            KindergartenFeeCompensation,
            SchoolMeals,
            MoscowStudentCard,
            FamilyMortgage,
            MatCapitalForHousing,
            LargeFamilyMortgagePayoff,
            FederalYoungFamilyHousingProgram,
            ChildTaxDeduction,
            EducationTaxDeduction,
            MedicalTaxDeduction,
            EmployerMaterialAidTaxBenefit,
            AnnualFamilyTaxPayment
        ];

        if (benefits.Length != 32)
        {
            throw new InvalidOperationException("The Moscow 2026 catalog must contain exactly 32 benefits.");
        }

        if (benefits.Any(static benefit => benefit is null))
        {
            throw new InvalidOperationException("The Moscow 2026 catalog contains an uninitialized benefit entry.");
        }

        return benefits.Cast<Benefit>().ToArray();
    }
}
