namespace Antro.Domain;

public sealed record Benefit
{
    public Benefit(
        BenefitId id,
        string slug,
        RegionCode region,
        CatalogVersion catalogVersion,
        BenefitType type,
        BenefitTier tier,
        BenefitCategory category,
        BenefitCopy copy,
        AmountRule amountRule,
        DeadlineRule? deadlineRule,
        IReadOnlyList<LifeStageAvailability> stageAvailability,
        IReadOnlyList<IEligibilityRule> eligibilityRules,
        IReadOnlyList<DocumentRequirement> documents,
        IReadOnlyList<ActionStep> actionSteps,
        IReadOnlyList<LegalSource> legalSources,
        PriorityHint? priorityHint = null)
    {
        Id = id;
        Slug = DomainGuard.RequiredText(slug, nameof(slug));
        Region = region;
        CatalogVersion = catalogVersion;
        Type = type;
        Tier = tier;
        Category = category;
        Copy = copy ?? throw new ArgumentNullException(nameof(copy));
        AmountRule = amountRule ?? throw new ArgumentNullException(nameof(amountRule));
        DeadlineRule = deadlineRule;
        StageAvailability = stageAvailability?.ToArray() ?? throw new ArgumentNullException(nameof(stageAvailability));
        EligibilityRules = eligibilityRules?.ToArray() ?? throw new ArgumentNullException(nameof(eligibilityRules));
        Documents = documents?.ToArray() ?? throw new ArgumentNullException(nameof(documents));
        ActionSteps = actionSteps?.ToArray() ?? throw new ArgumentNullException(nameof(actionSteps));
        LegalSources = legalSources?.ToArray() ?? throw new ArgumentNullException(nameof(legalSources));
        PriorityHint = priorityHint ?? PriorityHint.None;
    }

    public Benefit(
        BenefitId id,
        string slug,
        RegionCode region,
        BenefitType type,
        BenefitTier tier,
        BenefitCategory category,
        BenefitCopy copy,
        AmountRule amountRule,
        DeadlineRule? deadlineRule,
        IReadOnlyList<LifeStageAvailability> stageAvailability,
        IReadOnlyList<IEligibilityRule> eligibilityRules,
        IReadOnlyList<DocumentRequirement> documents,
        IReadOnlyList<ActionStep> actionSteps,
        IReadOnlyList<LegalSource> legalSources,
        PriorityHint? priorityHint = null)
        : this(
            id,
            slug,
            region,
            new CatalogVersion("unspecified"),
            type,
            tier,
            category,
            copy,
            amountRule,
            deadlineRule,
            stageAvailability,
            eligibilityRules,
            documents,
            actionSteps,
            legalSources,
            priorityHint)
    {
    }

    public BenefitId Id { get; }

    public string Slug { get; }

    public RegionCode Region { get; }

    public CatalogVersion CatalogVersion { get; }

    public BenefitType Type { get; }

    public BenefitTier Tier { get; }

    public BenefitCategory Category { get; }

    public BenefitCopy Copy { get; }

    public AmountRule AmountRule { get; }

    public DeadlineRule? DeadlineRule { get; }

    public IReadOnlyList<LifeStageAvailability> StageAvailability { get; }

    public IReadOnlyList<IEligibilityRule> EligibilityRules { get; }

    public IReadOnlyList<DocumentRequirement> Documents { get; }

    public IReadOnlyList<ActionStep> ActionSteps { get; }

    public IReadOnlyList<LegalSource> LegalSources { get; }

    public PriorityHint PriorityHint { get; }
}

public sealed record PriorityHint
{
    public static PriorityHint None { get; } = new();

    public PriorityHint(int hiddenGemScore = 0, int actionabilityBoost = 0)
    {
        if (hiddenGemScore < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(hiddenGemScore), "Hidden-gem score cannot be negative.");
        }

        if (actionabilityBoost < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(actionabilityBoost), "Actionability boost cannot be negative.");
        }

        HiddenGemScore = hiddenGemScore;
        ActionabilityBoost = actionabilityBoost;
    }

    public int HiddenGemScore { get; }

    public int ActionabilityBoost { get; }
}

public sealed record BenefitCopy
{
    public BenefitCopy(
        string title,
        string? officialTitle,
        string shortDescription,
        string detailedDescription,
        string? notes)
    {
        Title = DomainGuard.RequiredText(title, nameof(title));
        OfficialTitle = string.IsNullOrWhiteSpace(officialTitle) ? null : officialTitle.Trim();
        ShortDescription = DomainGuard.RequiredText(shortDescription, nameof(shortDescription));
        DetailedDescription = DomainGuard.RequiredText(detailedDescription, nameof(detailedDescription));
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    public string Title { get; }

    public string? OfficialTitle { get; }

    public string ShortDescription { get; }

    public string DetailedDescription { get; }

    public string? Notes { get; }
}

public sealed record DocumentRequirement
{
    public DocumentRequirement(string title, string? description, string? notes)
    {
        Title = DomainGuard.RequiredText(title, nameof(title));
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    public string Title { get; }

    public string? Description { get; }

    public string? Notes { get; }
}

public sealed record ActionStep
{
    public ActionStep(int order, string title, string? description, string? notes)
    {
        Order = order > 0 ? order : throw new ArgumentOutOfRangeException(nameof(order), "Action step order must be positive.");
        Title = DomainGuard.RequiredText(title, nameof(title));
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    public int Order { get; }

    public string Title { get; }

    public string? Description { get; }

    public string? Notes { get; }
}

public enum StageAvailabilityStatus
{
    Active = 0,
    Expired = 1,
    Future = 2,
    Informational = 3,
    StageDependent = 4
}

public sealed record LifeStageAvailability
{
    public LifeStageAvailability(
        LifeStage lifeStage,
        bool isVisible,
        StageAvailabilityStatus status,
        string? explanation)
    {
        LifeStage = lifeStage;
        IsVisible = isVisible;
        Status = status;
        Explanation = string.IsNullOrWhiteSpace(explanation) ? null : explanation.Trim();
    }

    public LifeStage LifeStage { get; }

    public bool IsVisible { get; }

    public StageAvailabilityStatus Status { get; }

    public string? Explanation { get; }
}
