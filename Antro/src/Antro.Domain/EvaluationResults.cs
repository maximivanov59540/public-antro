namespace Antro.Domain;

public enum AvailabilityStatus
{
    Available = 0,
    Unavailable = 1,
    Expired = 2,
    Automatic = 3,
    Informational = 4,
    NeedsMoreInfo = 5,
    PotentiallyAvailable = 6
}

public enum DeadlineStatus
{
    None = 0,
    Active = 1,
    Urgent = 2,
    Soon = 3,
    Expired = 4,
    Unknown = 5
}

public enum DeadlineKind
{
    None = 0,
    FilingDeadline = 1,
    ActiveUntil = 2,
    FixedPolicyEndDate = 3
}

public enum DisplayBadgeKind
{
    Status = 0,
    Deadline = 1,
    Automatic = 2,
    Informational = 3,
    Income = 4,
    MissingInfo = 5,
    Urgency = 6
}

public enum MoneyEstimateKind
{
    Exact = 0,
    Range = 1,
    UpTo = 2,
    Choice = 3,
    Percentage = 4,
    NaturalSupport = 5,
    StatusOnly = 6
}

public sealed record MoneyEstimateOption
{
    public MoneyEstimateOption(MoneyAmount amount, string? label = null)
    {
        Amount = amount;
        Label = string.IsNullOrWhiteSpace(label) ? null : label.Trim();
    }

    public MoneyAmount Amount { get; }

    public string? Label { get; }
}

public sealed record MoneyEstimate
{
    public MoneyEstimate(
        MoneyEstimateKind kind,
        AmountPeriod period,
        string displayText,
        bool canParticipateInTotals,
        MoneyAmount? exactAmount = null,
        MoneyAmount? minimumAmount = null,
        MoneyAmount? maximumAmount = null,
        decimal? percentage = null,
        MoneyAmount? capAmount = null,
        IReadOnlyList<MoneyEstimateOption>? options = null,
        IReadOnlyList<MissingProfileField>? missingFields = null,
        string? detailsText = null)
    {
        if (minimumAmount is not null && maximumAmount is not null)
        {
            if (minimumAmount.Value.Currency != maximumAmount.Value.Currency)
            {
                throw new ArgumentException("Money estimate bounds must use the same currency.", nameof(maximumAmount));
            }

            if (minimumAmount.Value.Period != maximumAmount.Value.Period)
            {
                throw new ArgumentException("Money estimate bounds must use the same period.", nameof(maximumAmount));
            }

            if (maximumAmount.Value.Value < minimumAmount.Value.Value)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumAmount), "Maximum amount cannot be less than minimum amount.");
            }
        }

        if (exactAmount is not null && minimumAmount is not null && exactAmount.Value != minimumAmount.Value)
        {
            throw new ArgumentException("Exact amount and minimum amount must match when both are provided.", nameof(minimumAmount));
        }

        if (exactAmount is not null && maximumAmount is not null && exactAmount.Value != maximumAmount.Value)
        {
            throw new ArgumentException("Exact amount and maximum amount must match when both are provided.", nameof(maximumAmount));
        }

        MinimumAmount = minimumAmount;
        MaximumAmount = maximumAmount;
        Kind = kind;
        Period = period;
        DisplayText = DomainGuard.RequiredText(displayText, nameof(displayText));
        CanParticipateInTotals = canParticipateInTotals;
        ExactAmount = exactAmount;
        Percentage = percentage;
        CapAmount = capAmount;
        Options = options?.ToArray() ?? Array.Empty<MoneyEstimateOption>();
        MissingFields = missingFields?.Distinct().ToArray() ?? Array.Empty<MissingProfileField>();
        DetailsText = string.IsNullOrWhiteSpace(detailsText) ? null : detailsText.Trim();
    }

    public MoneyEstimateKind Kind { get; }

    public AmountPeriod Period { get; }

    public string DisplayText { get; }

    public bool CanParticipateInTotals { get; }

    public MoneyAmount? ExactAmount { get; }

    public MoneyAmount? MinimumAmount { get; }

    public MoneyAmount? MaximumAmount { get; }

    public decimal? Percentage { get; }

    public MoneyAmount? CapAmount { get; }

    public IReadOnlyList<MoneyEstimateOption> Options { get; }

    public IReadOnlyList<MissingProfileField> MissingFields { get; }

    public string? DetailsText { get; }

    public bool IsExact => Kind == MoneyEstimateKind.Exact;

    public bool IsMonetary => Kind is not MoneyEstimateKind.NaturalSupport and not MoneyEstimateKind.StatusOnly;
}

public sealed record DisplayBadge
{
    public DisplayBadge(DisplayBadgeKind kind, string text, string semanticKey)
    {
        Kind = kind;
        Text = DomainGuard.RequiredText(text, nameof(text));
        SemanticKey = DomainGuard.RequiredText(semanticKey, nameof(semanticKey));
    }

    public DisplayBadgeKind Kind { get; }

    public string Text { get; }

    public string SemanticKey { get; }
}

public sealed record DeadlineEvaluation
{
    public DeadlineEvaluation(
        DeadlineStatus status,
        DeadlineKind kind,
        DateOnly? deadlineDate,
        int? daysLeft,
        string shortText,
        string explanation)
    {
        Status = status;
        Kind = kind;
        DeadlineDate = deadlineDate;
        DaysLeft = daysLeft;
        ShortText = DomainGuard.RequiredText(shortText, nameof(shortText));
        Explanation = DomainGuard.RequiredText(explanation, nameof(explanation));
    }

    public DeadlineStatus Status { get; }

    public DeadlineKind Kind { get; }

    public DateOnly? DeadlineDate { get; }

    public int? DaysLeft { get; }

    public string ShortText { get; }

    public string Explanation { get; }
}

public sealed record EvaluatedBenefit
{
    public EvaluatedBenefit(
        BenefitId benefitId,
        string benefitSlug,
        BenefitType benefitType,
        BenefitTier benefitTier,
        AvailabilityStatus availabilityStatus,
        DeadlineEvaluation deadline,
        MoneyEstimate? moneyEstimate,
        int priorityScore,
        string? userFacingReason,
        IReadOnlyList<MissingProfileField>? missingFields,
        IReadOnlyList<DisplayBadge>? badges,
        PriorityHint? priorityHint = null)
    {
        BenefitId = benefitId;
        BenefitSlug = DomainGuard.RequiredText(benefitSlug, nameof(benefitSlug));
        BenefitType = benefitType;
        BenefitTier = benefitTier;
        AvailabilityStatus = availabilityStatus;
        Deadline = deadline ?? throw new ArgumentNullException(nameof(deadline));
        MoneyEstimate = moneyEstimate;
        PriorityScore = priorityScore;
        UserFacingReason = string.IsNullOrWhiteSpace(userFacingReason) ? null : userFacingReason.Trim();
        MissingFields = missingFields?.ToArray() ?? Array.Empty<MissingProfileField>();
        Badges = badges?.ToArray() ?? Array.Empty<DisplayBadge>();
        PriorityHint = priorityHint ?? PriorityHint.None;
    }

    public BenefitId BenefitId { get; }

    public string BenefitSlug { get; }

    public BenefitType BenefitType { get; }

    public BenefitTier BenefitTier { get; }

    public AvailabilityStatus AvailabilityStatus { get; }

    public DeadlineEvaluation Deadline { get; }

    public DeadlineStatus DeadlineStatus => Deadline.Status;

    public MoneyEstimate? MoneyEstimate { get; }

    public int PriorityScore { get; }

    public string? UserFacingReason { get; }

    public IReadOnlyList<MissingProfileField> MissingFields { get; }

    public IReadOnlyList<DisplayBadge> Badges { get; }

    public PriorityHint PriorityHint { get; }
}
