using Antro.Domain;

namespace Antro.Content.Moscow2026;

internal static class Moscow2026EligibilityRules
{
    private static readonly IEligibilityRule RussianCitizenship = new AssumedRussianCitizenshipRule();
    private static readonly IEligibilityRule MoscowRegistration = new AssumedMoscowRegistrationRule();
    private static readonly IEligibilityRule LowIncome = new IncomeBandRule(
        [IncomeBand.UpToOneLivingMinimum, IncomeBand.UpToOneAndHalfLivingMinimum, IncomeBand.UpToTwoLivingMinimums]);
    // Единое пособие: среднедушевой доход семьи строго ниже 1 ПМ на душу населения (не 2 ПМ).
    private static readonly IEligibilityRule UnifiedBenefitLowIncome = new IncomeBandRule(
        [IncomeBand.UpToOneLivingMinimum],
        failExplanation: "Единое пособие назначается при среднедушевом доходе ниже одного прожиточного минимума на душу населения.",
        unknownExplanation: "Для единого пособия нужно уточнить доходный диапазон семьи (порог — ниже 1 ПМ на душу).");
    private static readonly IEligibilityRule AnyKnownIncome = new IncomeBandRule(
        [IncomeBand.UpToOneLivingMinimum, IncomeBand.UpToOneAndHalfLivingMinimum, IncomeBand.UpToTwoLivingMinimums, IncomeBand.AboveTwoLivingMinimums],
        failExplanation: "Для этой меры нужно проверить доходный диапазон семьи.",
        unknownExplanation: "Для этой меры нужно уточнить доходный диапазон семьи.");
    private static readonly IEligibilityRule AnnualFamilyLowIncome = new IncomeBandRule(
        [IncomeBand.UpToOneLivingMinimum, IncomeBand.UpToOneAndHalfLivingMinimum],
        failExplanation: "Сейчас эта мера не подходит по выбранному доходному диапазону.",
        unknownExplanation: "Эта ежегодная выплата зависит от дохода семьи. Уточните доходный диапазон.");
    private static readonly IEligibilityRule YoungParentAge = new ParentAgeBandRule(
        [ParentAgeBand.BothUnderThirtySix]);
    private static readonly IEligibilityRule EmployedOnly = new EmploymentStatusRule(
        [EmploymentStatus.MotherOnly, EmploymentStatus.Both]);
    private static readonly IEligibilityRule EmployedOrSelfEmployed = new EmploymentStatusRule(
        [EmploymentStatus.MotherOnly, EmploymentStatus.FatherOnly, EmploymentStatus.Both],
        failExplanation: "Для этой меры нужен подтверждённый трудовой статус родителя.");
    private static readonly IEligibilityRule PropertyNeedsReview = new PropertyStatusRule(
        [PropertyStatus.DoesNotOwnHome, PropertyStatus.OwnsHome],
        unknownExplanation: "Для этой меры нужно уточнить жилищный статус семьи.");
    private static readonly IEligibilityRule HousingNeed = new PropertyStatusRule(
        [PropertyStatus.DoesNotOwnHome],
        failExplanation: "Эта программа обычно ориентирована на семьи, которым нужно улучшение жилищных условий.",
        unknownExplanation: "Для этой программы нужно уточнить жилищный статус семьи.");
    private static readonly IEligibilityRule MortgageRelevant = new MortgageIntentRule(
        [MortgageIntent.PlansToUseMortgage, MortgageIntent.HasActiveMortgage]);
    private static readonly IEligibilityRule MatCapitalAvailable = new MatCapitalHistoryRule(
        [MatCapitalHistory.NeverReceived, MatCapitalHistory.ReceivedUnused, MatCapitalHistory.PartiallyUsed]);

    public static IReadOnlyList<IEligibilityRule> PregnancyMonthlyBenefit() =>
    [
        RussianCitizenship,
        UnifiedBenefitLowIncome
    ];

    public static IReadOnlyList<IEligibilityRule> MaternityLeaveBenefit() =>
    [
        AnyKnownIncome,
        EmployedOrSelfEmployed
    ];

    public static IReadOnlyList<IEligibilityRule> MoscowRegistrationOnly() =>
    [
        MoscowRegistration
    ];

    public static IReadOnlyList<IEligibilityRule> PregnancyEmploymentRight() =>
    [
        EmployedOnly
    ];

    public static IReadOnlyList<IEligibilityRule> FederalBirthGrant() =>
    [
        RussianCitizenship
    ];

    public static IReadOnlyList<IEligibilityRule> MoscowYoungFamilyPayment() =>
    [
        MoscowRegistration,
        YoungParentAge
    ];

    public static IReadOnlyList<IEligibilityRule> MaternityCapital() =>
    [
        RussianCitizenship,
        MatCapitalAvailable
    ];

    // Пособие по уходу до 1,5 лет положено и работающим (страховой контур), и неработающим (маршрут СФР),
    // поэтому занятость не должна блокировать карточку — для неработающих выплачивается минимум.
    public static IReadOnlyList<IEligibilityRule> ChildCareMonthlyAllowance() =>
    [
        AnyKnownIncome
    ];

    public static IReadOnlyList<IEligibilityRule> KindergartenFeeCompensation() =>
    [
        AnyKnownIncome,
        MoscowRegistration
    ];

    public static IReadOnlyList<IEligibilityRule> UnifiedChildBenefit() =>
    [
        RussianCitizenship,
        UnifiedBenefitLowIncome,
        PropertyNeedsReview
    ];

    public static IReadOnlyList<IEligibilityRule> MatCapitalMonthlyPayment() =>
    [
        RussianCitizenship,
        LowIncome,
        MatCapitalAvailable
    ];

    public static IReadOnlyList<IEligibilityRule> ChildcareEmploymentRight() =>
    [
        EmployedOnly
    ];

    public static IReadOnlyList<IEligibilityRule> FamilyMortgage() =>
    [
        MortgageRelevant
    ];

    public static IReadOnlyList<IEligibilityRule> MatCapitalForHousing() =>
    [
        MatCapitalAvailable
    ];

    public static IReadOnlyList<IEligibilityRule> FederalYoungFamilyHousingProgram() =>
    [
        YoungParentAge,
        HousingNeed,
        MortgageRelevant
    ];

    public static IReadOnlyList<IEligibilityRule> TaxBenefitForWorkingParent() =>
    [
        EmployedOnly
    ];

    public static IReadOnlyList<IEligibilityRule> AnnualFamilyTaxPayment() =>
    [
        RussianCitizenship,
        AnnualFamilyLowIncome
    ];

    // 450 000 ₽ многодетным на погашение ипотеки: заявитель — заёмщик/созаёмщик по ипотеке,
    // гражданин РФ; условие «третий и последующий ребёнок 2019–2030» проверяется по копирайту,
    // так как модель пока не содержит отдельного правила по очерёдности ребёнка.
    public static IReadOnlyList<IEligibilityRule> LargeFamilyMortgagePayoff() =>
    [
        RussianCitizenship,
        MortgageRelevant
    ];
}
