using Antro.Domain;

namespace Antro.Application;

internal static class BenefitViewModelMapper
{
    public static BenefitCardViewModel ToCard(Benefit benefit, EvaluatedBenefit evaluatedBenefit)
    {
        var sectionKey = ApplicationDisplayText.GetSectionCategoryKey(
            evaluatedBenefit.Deadline.Status,
            benefit.Type);

        return new(
            benefit.Id,
            benefit.Slug,
            $"/benefits/{benefit.Slug}",
            benefit.Copy.Title,
            benefit.Copy.ShortDescription,
            ApplicationDisplayText.GetCategoryKey(benefit.Category),
            ApplicationDisplayText.GetAvailabilityPill(evaluatedBenefit.AvailabilityStatus),
            ToAmountDisplay(evaluatedBenefit.MoneyEstimate),
            ToDeadlineBadge(evaluatedBenefit.Deadline),
            evaluatedBenefit.UserFacingReason ?? benefit.Copy.ShortDescription,
            evaluatedBenefit.PriorityScore,
            evaluatedBenefit.Badges.Select(badge => new StatusPillViewModel(badge.Text, badge.SemanticKey)).ToArray(),
            IsMuted(evaluatedBenefit.AvailabilityStatus),
            ApplicationDisplayText.GetIconKey(benefit.Category, benefit.Type),
            sectionKey,
            benefit.Category);
    }

    public static AmountDisplayViewModel? ToAmountDisplay(MoneyEstimate? moneyEstimate)
    {
        if (moneyEstimate is null)
        {
            return null;
        }

        return new AmountDisplayViewModel(
            moneyEstimate.DisplayText,
            GetMoneyKindKey(moneyEstimate.Kind),
            moneyEstimate.IsMonetary,
            moneyEstimate.CanParticipateInTotals,
            moneyEstimate.DetailsText);
    }

    public static DeadlineBadgeViewModel ToDeadlineBadge(DeadlineEvaluation deadline) =>
        new(
            deadline.ShortText,
            GetDeadlineStatusKey(deadline.Status),
            GetDeadlineKindKey(deadline.Kind),
            deadline.DeadlineDate,
            deadline.DaysLeft,
            deadline.Explanation);

    public static DeadlinePreviewViewModel ToDeadlinePreview(Benefit benefit, EvaluatedBenefit evaluatedBenefit) =>
        new(
            benefit.Id,
            $"/benefits/{benefit.Slug}",
            benefit.Copy.Title,
            ApplicationDisplayText.GetAvailabilityPill(evaluatedBenefit.AvailabilityStatus),
            ToDeadlineBadge(evaluatedBenefit.Deadline));

    public static RightPreviewViewModel ToRightPreview(Benefit benefit, EvaluatedBenefit evaluatedBenefit) =>
        new(
            benefit.Id,
            $"/benefits/{benefit.Slug}",
            benefit.Copy.Title,
            benefit.Copy.ShortDescription,
            ApplicationDisplayText.GetAvailabilityPill(evaluatedBenefit.AvailabilityStatus));

    private static bool IsMuted(AvailabilityStatus status) =>
        status is AvailabilityStatus.Unavailable or AvailabilityStatus.Expired;

    private static string GetMoneyKindKey(MoneyEstimateKind kind) =>
        kind switch
        {
            MoneyEstimateKind.Exact => "exact",
            MoneyEstimateKind.Range => "range",
            MoneyEstimateKind.UpTo => "up-to",
            MoneyEstimateKind.Choice => "choice",
            MoneyEstimateKind.Percentage => "percentage",
            MoneyEstimateKind.NaturalSupport => "natural-support",
            MoneyEstimateKind.StatusOnly => "status-only",
            _ => "amount"
        };

    private static string GetDeadlineStatusKey(DeadlineStatus status) =>
        status switch
        {
            DeadlineStatus.None => "none",
            DeadlineStatus.Active => "active",
            DeadlineStatus.Urgent => "urgent",
            DeadlineStatus.Soon => "soon",
            DeadlineStatus.Expired => "expired",
            DeadlineStatus.Unknown => "unknown",
            _ => "deadline"
        };

    private static string GetDeadlineKindKey(DeadlineKind kind) =>
        kind switch
        {
            DeadlineKind.None => "none",
            DeadlineKind.FilingDeadline => "filing-deadline",
            DeadlineKind.ActiveUntil => "active-until",
            DeadlineKind.FixedPolicyEndDate => "fixed-policy-end-date",
            _ => "deadline-kind"
        };
}
