using Antro.Domain;

namespace Antro.Application;

public sealed class BenefitPrioritySorter : IBenefitPrioritySorter
{
    public IReadOnlyList<EvaluatedBenefit> Sort(IReadOnlyList<EvaluatedBenefit> benefits)
    {
        if (benefits is null)
        {
            throw new ArgumentNullException(nameof(benefits));
        }

        return benefits
            .Select(ApplyPriorityScore)
            .OrderByDescending(benefit => benefit.PriorityScore)
            .ThenBy(benefit => GetTierFallbackRank(benefit.BenefitTier))
            .ThenBy(benefit => benefit.BenefitSlug, StringComparer.Ordinal)
            .ThenBy(benefit => benefit.BenefitId.Value, StringComparer.Ordinal)
            .ToArray();
    }

    private static EvaluatedBenefit ApplyPriorityScore(EvaluatedBenefit benefit)
    {
        var score = 0;

        // The MVP sorts by actionability and risk of loss first, not by nominal amount.
        // Deadline pressure and hidden-gem hints therefore outweigh raw ruble size.
        score += benefit.AvailabilityStatus switch
        {
            AvailabilityStatus.Available => 500,
            AvailabilityStatus.PotentiallyAvailable => 430,
            AvailabilityStatus.Automatic => 250,
            AvailabilityStatus.NeedsMoreInfo => 220,
            AvailabilityStatus.Informational => 160,
            AvailabilityStatus.Unavailable => 100,
            AvailabilityStatus.Expired => 40,
            _ => 0
        };

        score += benefit.DeadlineStatus switch
        {
            DeadlineStatus.Urgent => 220,
            DeadlineStatus.Soon => 140,
            DeadlineStatus.Active => 80,
            DeadlineStatus.Unknown => -30,
            DeadlineStatus.Expired => -220,
            _ => 0
        };

        score += benefit.PriorityHint.HiddenGemScore * 70;
        score += benefit.PriorityHint.ActionabilityBoost * 45;

        score += benefit.BenefitType switch
        {
            BenefitType.Automatic => -80,
            BenefitType.InformationalRight => -50,
            _ => 0
        };

        if (benefit.AvailabilityStatus == AvailabilityStatus.Automatic)
        {
            score -= 160;
        }

        if (benefit.AvailabilityStatus == AvailabilityStatus.NeedsMoreInfo)
        {
            score -= 60;
        }

        if (benefit.AvailabilityStatus == AvailabilityStatus.Unavailable)
        {
            score -= 80;
        }

        if (benefit.AvailabilityStatus == AvailabilityStatus.Expired)
        {
            score -= 200;
        }

        if (benefit.MoneyEstimate is null)
        {
            return CopyWithScore(benefit, score);
        }

        score += benefit.MoneyEstimate.Kind switch
        {
            MoneyEstimateKind.NaturalSupport => 15,
            MoneyEstimateKind.StatusOnly => -20,
            MoneyEstimateKind.Percentage => -10,
            _ => 0
        };

        if (!benefit.MoneyEstimate.CanParticipateInTotals)
        {
            score -= 10;
        }

        return CopyWithScore(benefit, score);
    }

    private static EvaluatedBenefit CopyWithScore(EvaluatedBenefit benefit, int priorityScore) =>
        new(
            benefit.BenefitId,
            benefit.BenefitSlug,
            benefit.BenefitType,
            benefit.BenefitTier,
            benefit.AvailabilityStatus,
            benefit.Deadline,
            benefit.MoneyEstimate,
            priorityScore,
            benefit.UserFacingReason,
            benefit.MissingFields,
            benefit.Badges,
            benefit.PriorityHint);

    private static int GetTierFallbackRank(BenefitTier tier) =>
        tier switch
        {
            BenefitTier.Tier1Core => 0,
            BenefitTier.Tier2Value => 1,
            BenefitTier.Tier3HiddenGem => 2,
            _ => 3
        };
}
