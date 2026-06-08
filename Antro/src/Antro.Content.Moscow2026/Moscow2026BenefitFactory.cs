using Antro.Domain;

namespace Antro.Content.Moscow2026;

internal static class Moscow2026BenefitFactory
{
    public static Benefit Create(
        BenefitId id,
        string slug,
        BenefitType type,
        BenefitTier tier,
        BenefitCategory category,
        BenefitCopy copy,
        AmountRule amountRule,
        DeadlineRule? deadlineRule,
        IReadOnlyList<LifeStageAvailability> stageAvailability,
        IReadOnlyList<LegalSource> legalSources,
        IReadOnlyList<DocumentRequirement> documents,
        IReadOnlyList<ActionStep> actionSteps,
        IReadOnlyList<IEligibilityRule>? eligibilityRules = null,
        PriorityHint? priorityHint = null) =>
        new(
            id,
            slug,
            Moscow2026BenefitCatalog.Region,
            Moscow2026BenefitCatalog.CatalogVersion,
            type,
            tier,
            category,
            copy,
            amountRule,
            deadlineRule,
            stageAvailability,
            eligibilityRules ?? Array.Empty<IEligibilityRule>(),
            documents,
            actionSteps,
            legalSources,
            priorityHint);
}
