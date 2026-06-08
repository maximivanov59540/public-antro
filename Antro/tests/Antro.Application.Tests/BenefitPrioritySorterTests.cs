using Antro.Application;
using Antro.Domain;

namespace Antro.Application.Tests;

public sealed class BenefitPrioritySorterTests
{
    private readonly IBenefitPrioritySorter sorter = new BenefitPrioritySorter();

    [Fact]
    public void Urgent_available_benefit_ranks_above_large_automatic_benefit()
    {
        var urgentAction = CreateEvaluatedBenefit(
            "urgent-action",
            BenefitType.DeadlineEvent,
            BenefitTier.Tier2Value,
            AvailabilityStatus.Available,
            new DeadlineEvaluation(
                DeadlineStatus.Urgent,
                DeadlineKind.FilingDeadline,
                new DateOnly(2026, 5, 25),
                2,
                "2 days left",
                "Urgent filing deadline."),
            new MoneyEstimate(
                MoneyEstimateKind.NaturalSupport,
                AmountPeriod.NotApplicable,
                "Natural support",
                canParticipateInTotals: false),
            new PriorityHint(hiddenGemScore: 3, actionabilityBoost: 3));

        var automaticLargeAmount = CreateEvaluatedBenefit(
            "automatic-large-amount",
            BenefitType.Automatic,
            BenefitTier.Tier1Core,
            AvailabilityStatus.Automatic,
            new DeadlineEvaluation(
                DeadlineStatus.None,
                DeadlineKind.None,
                null,
                null,
                "No deadline",
                "Automatic benefit."),
            new MoneyEstimate(
                MoneyEstimateKind.Exact,
                AmountPeriod.OneTime,
                "900 000 \u20BD",
                canParticipateInTotals: true,
                exactAmount: new MoneyAmount(900_000m, Currency.Rub, AmountPeriod.OneTime)));

        var sorted = sorter.Sort([automaticLargeAmount, urgentAction]);

        Assert.Equal("urgent-action", sorted[0].BenefitSlug);
        Assert.True(sorted[0].PriorityScore > sorted[1].PriorityScore);
    }

    [Fact]
    public void Needs_more_info_benefit_stays_visible_but_below_urgent_available_benefit()
    {
        var urgentKnownBenefit = CreateEvaluatedBenefit(
            "urgent-known",
            BenefitType.DeadlineEvent,
            BenefitTier.Tier1Core,
            AvailabilityStatus.Available,
            new DeadlineEvaluation(
                DeadlineStatus.Soon,
                DeadlineKind.FilingDeadline,
                new DateOnly(2026, 6, 1),
                9,
                "9 days left",
                "Deadline is approaching."));

        var needsMoreInfo = CreateEvaluatedBenefit(
            "needs-info",
            BenefitType.IncomeDependentPayment,
            BenefitTier.Tier1Core,
            AvailabilityStatus.NeedsMoreInfo,
            new DeadlineEvaluation(
                DeadlineStatus.None,
                DeadlineKind.None,
                null,
                null,
                "No deadline",
                "Need income details."),
            new MoneyEstimate(
                MoneyEstimateKind.Range,
                AmountPeriod.Monthly,
                "10 000\u201320 000 \u20BD/\u043c\u0435\u0441",
                canParticipateInTotals: false,
                minimumAmount: new MoneyAmount(10_000m, Currency.Rub, AmountPeriod.Monthly),
                maximumAmount: new MoneyAmount(20_000m, Currency.Rub, AmountPeriod.Monthly)),
            missingFields: [MissingProfileField.IncomeBand]);

        var sorted = sorter.Sort([needsMoreInfo, urgentKnownBenefit]);

        Assert.Equal("urgent-known", sorted[0].BenefitSlug);
        Assert.Equal("needs-info", sorted[1].BenefitSlug);
    }

    [Fact]
    public void Expired_benefit_ranks_lower_but_remains_present()
    {
        var expiredBenefit = CreateEvaluatedBenefit(
            "expired-benefit",
            BenefitType.DeadlineEvent,
            BenefitTier.Tier2Value,
            AvailabilityStatus.Expired,
            new DeadlineEvaluation(
                DeadlineStatus.Expired,
                DeadlineKind.FilingDeadline,
                new DateOnly(2026, 5, 1),
                -22,
                "Deadline passed",
                "Deadline has passed."),
            new MoneyEstimate(
                MoneyEstimateKind.NaturalSupport,
                AmountPeriod.NotApplicable,
                "Natural support",
                canParticipateInTotals: false),
            new PriorityHint(hiddenGemScore: 2, actionabilityBoost: 2));

        var activeBenefit = CreateEvaluatedBenefit(
            "active-benefit",
            BenefitType.DeadlineEvent,
            BenefitTier.Tier1Core,
            AvailabilityStatus.Available,
            new DeadlineEvaluation(
                DeadlineStatus.Active,
                DeadlineKind.FilingDeadline,
                new DateOnly(2026, 6, 20),
                28,
                "28 days left",
                "Still active."));

        var sorted = sorter.Sort([expiredBenefit, activeBenefit]);

        Assert.Equal(["active-benefit", "expired-benefit"], sorted.Select(result => result.BenefitSlug).ToArray());
    }

    [Fact]
    public void Equal_scores_use_deterministic_tier_and_slug_fallback()
    {
        var tierTwo = CreateEvaluatedBenefit(
            "z-tier-two",
            BenefitType.OngoingRight,
            BenefitTier.Tier2Value,
            AvailabilityStatus.Available,
            new DeadlineEvaluation(DeadlineStatus.None, DeadlineKind.None, null, null, "No deadline", "No deadline."));

        var tierOneB = CreateEvaluatedBenefit(
            "z-tier-one",
            BenefitType.OngoingRight,
            BenefitTier.Tier1Core,
            AvailabilityStatus.Available,
            new DeadlineEvaluation(DeadlineStatus.None, DeadlineKind.None, null, null, "No deadline", "No deadline."));

        var tierOneA = CreateEvaluatedBenefit(
            "a-tier-one",
            BenefitType.OngoingRight,
            BenefitTier.Tier1Core,
            AvailabilityStatus.Available,
            new DeadlineEvaluation(DeadlineStatus.None, DeadlineKind.None, null, null, "No deadline", "No deadline."));

        var sorted = sorter.Sort([tierTwo, tierOneB, tierOneA]);

        Assert.Equal(["a-tier-one", "z-tier-one", "z-tier-two"], sorted.Select(result => result.BenefitSlug).ToArray());
        Assert.Equal(sorted[0].PriorityScore, sorted[1].PriorityScore);
        Assert.Equal(sorted[1].PriorityScore, sorted[2].PriorityScore);
    }

    private static EvaluatedBenefit CreateEvaluatedBenefit(
        string slug,
        BenefitType benefitType,
        BenefitTier benefitTier,
        AvailabilityStatus availabilityStatus,
        DeadlineEvaluation deadline,
        MoneyEstimate? moneyEstimate = null,
        PriorityHint? priorityHint = null,
        IReadOnlyList<MissingProfileField>? missingFields = null) =>
        new(
            new BenefitId(slug),
            slug,
            benefitType,
            benefitTier,
            availabilityStatus,
            deadline,
            moneyEstimate,
            priorityScore: 0,
            userFacingReason: null,
            missingFields,
            badges: Array.Empty<DisplayBadge>(),
            priorityHint: priorityHint);
}
