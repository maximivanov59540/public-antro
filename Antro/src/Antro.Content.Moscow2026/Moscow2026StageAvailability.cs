using Antro.Domain;

namespace Antro.Content.Moscow2026;

internal static class Moscow2026StageAvailability
{
    public static IReadOnlyList<LifeStageAvailability> PregnancyOnly(string expiredExplanation) =>
    [
        Active(LifeStage.ExpectingChild, "Relevant during pregnancy."),
        Expired(LifeStage.NewbornZeroToTwoMonths, expiredExplanation)
    ];

    public static IReadOnlyList<LifeStageAvailability> PregnancyWithEarlyPostpartumContext(string postpartumExplanation) =>
    [
        Active(LifeStage.ExpectingChild, "Relevant during pregnancy."),
        Informational(LifeStage.NewbornZeroToTwoMonths, postpartumExplanation)
    ];

    public static IReadOnlyList<LifeStageAvailability> NewbornTwoMonthDeadline(string expiredExplanation) =>
    [
        Active(LifeStage.NewbornZeroToTwoMonths, "Normally active during the first two months after birth."),
        Expired(LifeStage.UpToSixMonths, expiredExplanation)
    ];

    public static IReadOnlyList<LifeStageAvailability> NewbornSixMonthDeadline(string expiredExplanation) =>
    [
        Active(LifeStage.NewbornZeroToTwoMonths, "Normally active during the first six months after birth."),
        Active(LifeStage.UpToSixMonths, "Still normally active while the six-month filing window remains open."),
        Expired(LifeStage.UpToEighteenMonths, expiredExplanation)
    ];

    public static IReadOnlyList<LifeStageAvailability> NewbornTwelveMonthDeadline(string stageDependentExplanation) =>
    [
        Active(LifeStage.NewbornZeroToTwoMonths, "Normally active during the first year after birth."),
        Active(LifeStage.UpToSixMonths, "Still normally active while the one-year filing window remains open."),
        StageDependent(LifeStage.UpToEighteenMonths, stageDependentExplanation)
    ];

    public static IReadOnlyList<LifeStageAvailability> UntilEighteenMonths(string expiredExplanation) =>
    [
        Active(LifeStage.NewbornZeroToTwoMonths, "Normally active for infants."),
        Active(LifeStage.UpToSixMonths, "Normally active during early infancy."),
        Active(LifeStage.UpToEighteenMonths, "Normally active until the child reaches 1.5 years."),
        Expired(LifeStage.UpToThreeYears, expiredExplanation)
    ];

    public static IReadOnlyList<LifeStageAvailability> UntilThreeYears(string expiredExplanation) =>
    [
        Active(LifeStage.NewbornZeroToTwoMonths, "Normally active after birth."),
        Active(LifeStage.UpToSixMonths, "Normally active during early infancy."),
        Active(LifeStage.UpToEighteenMonths, "Normally active during toddler years."),
        Active(LifeStage.UpToThreeYears, "Normally active until the child reaches three years."),
        Expired(LifeStage.OlderThanThreeYears, expiredExplanation)
    ];

    public static IReadOnlyList<LifeStageAvailability> PostBirthAllStages(string olderChildExplanation) =>
    [
        Active(LifeStage.NewbornZeroToTwoMonths, "Relevant immediately after birth."),
        Active(LifeStage.UpToSixMonths, "Relevant during infancy."),
        Active(LifeStage.UpToEighteenMonths, "Relevant during toddler years."),
        Active(LifeStage.UpToThreeYears, "Relevant for young children."),
        Informational(LifeStage.OlderThanThreeYears, olderChildExplanation)
    ];

    public static IReadOnlyList<LifeStageAvailability> AllChildStagesActive(string olderChildExplanation) =>
    [
        Active(LifeStage.NewbornZeroToTwoMonths, "Relevant immediately after birth."),
        Active(LifeStage.UpToSixMonths, "Relevant during infancy."),
        Active(LifeStage.UpToEighteenMonths, "Relevant during toddler years."),
        Active(LifeStage.UpToThreeYears, "Relevant for young children."),
        Active(LifeStage.OlderThanThreeYears, olderChildExplanation)
    ];

    public static IReadOnlyList<LifeStageAvailability> PreschoolAndOlder() =>
    [
        Future(LifeStage.UpToEighteenMonths, "Usually relevant later, closer to preschool or school age."),
        Active(LifeStage.UpToThreeYears, "Relevant as preschool planning starts."),
        Active(LifeStage.OlderThanThreeYears, "Still relevant for older children.")
    ];

    public static IReadOnlyList<LifeStageAvailability> SchoolAge() =>
    [
        Future(LifeStage.UpToThreeYears, "Usually becomes relevant later, once the child approaches school age."),
        Active(LifeStage.OlderThanThreeYears, "Relevant for school-age children.")
    ];

    public static IReadOnlyList<LifeStageAvailability> HousingLongTail(string expectingExplanation, string olderChildExplanation) =>
    [
        Informational(LifeStage.ExpectingChild, expectingExplanation),
        Active(LifeStage.NewbornZeroToTwoMonths, "Relevant once the family has a child or confirmed pregnancy criteria."),
        Active(LifeStage.UpToSixMonths, "Relevant while housing planning is active."),
        Active(LifeStage.UpToEighteenMonths, "Relevant while housing planning is active."),
        Active(LifeStage.UpToThreeYears, "Relevant while housing planning is active."),
        Informational(LifeStage.OlderThanThreeYears, olderChildExplanation)
    ];

    public static IReadOnlyList<LifeStageAvailability> EmployerBirthYearTaxBenefit() =>
    [
        Active(LifeStage.NewbornZeroToTwoMonths, "Relevant soon after birth if the employer provides material aid."),
        Active(LifeStage.UpToSixMonths, "Usually still relevant during the first year."),
        StageDependent(LifeStage.UpToEighteenMonths, "Draft: tax treatment is usually tied to support paid within the first year after birth."),
        Expired(LifeStage.UpToThreeYears, "Normally no longer relevant once the first-year timing has passed.")
    ];

    private static LifeStageAvailability Active(LifeStage lifeStage, string explanation) =>
        new(lifeStage, true, StageAvailabilityStatus.Active, explanation);

    private static LifeStageAvailability Expired(LifeStage lifeStage, string explanation) =>
        new(lifeStage, true, StageAvailabilityStatus.Expired, explanation);

    private static LifeStageAvailability Future(LifeStage lifeStage, string explanation) =>
        new(lifeStage, true, StageAvailabilityStatus.Future, explanation);

    private static LifeStageAvailability Informational(LifeStage lifeStage, string explanation) =>
        new(lifeStage, true, StageAvailabilityStatus.Informational, explanation);

    private static LifeStageAvailability StageDependent(LifeStage lifeStage, string explanation) =>
        new(lifeStage, true, StageAvailabilityStatus.StageDependent, explanation);
}
