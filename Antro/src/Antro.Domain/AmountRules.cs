namespace Antro.Domain;

public abstract record AmountRule;

public sealed record FixedAmount(MoneyAmount Amount) : AmountRule;

public sealed record AmountRange : AmountRule
{
    public AmountRange(MoneyAmount minimumAmount, MoneyAmount maximumAmount)
    {
        if (minimumAmount.Currency != maximumAmount.Currency)
        {
            throw new ArgumentException("Amount range must use the same currency for both bounds.", nameof(maximumAmount));
        }

        if (minimumAmount.Period != maximumAmount.Period)
        {
            throw new ArgumentException("Amount range must use the same period for both bounds.", nameof(maximumAmount));
        }

        if (maximumAmount.Value < minimumAmount.Value)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumAmount), "Maximum amount cannot be less than minimum amount.");
        }

        MinimumAmount = minimumAmount;
        MaximumAmount = maximumAmount;
    }

    public MoneyAmount MinimumAmount { get; }

    public MoneyAmount MaximumAmount { get; }
}

public sealed record UpToAmount(MoneyAmount MaximumAmount) : AmountRule;

public sealed record ChoiceAmountOption
{
    public ChoiceAmountOption(
        MoneyAmount amount,
        string? label = null,
        ChildOrder? childOrder = null,
        MatCapitalHistory? matCapitalHistory = null,
        FamilyStatus? familyStatus = null,
        EmploymentStatus? employmentStatus = null,
        ParentAgeBand? parentAgeBand = null,
        IncomeBand? incomeBand = null,
        PropertyStatus? propertyStatus = null,
        MortgageIntent? mortgageIntent = null)
    {
        Amount = amount;
        Label = string.IsNullOrWhiteSpace(label) ? null : label.Trim();
        ChildOrder = childOrder;
        MatCapitalHistory = matCapitalHistory;
        FamilyStatus = familyStatus;
        EmploymentStatus = employmentStatus;
        ParentAgeBand = parentAgeBand;
        IncomeBand = incomeBand;
        PropertyStatus = propertyStatus;
        MortgageIntent = mortgageIntent;
    }

    public MoneyAmount Amount { get; }

    public string? Label { get; }

    public ChildOrder? ChildOrder { get; }

    public MatCapitalHistory? MatCapitalHistory { get; }

    public FamilyStatus? FamilyStatus { get; }

    public EmploymentStatus? EmploymentStatus { get; }

    public ParentAgeBand? ParentAgeBand { get; }

    public IncomeBand? IncomeBand { get; }

    public PropertyStatus? PropertyStatus { get; }

    public MortgageIntent? MortgageIntent { get; }
}

public sealed record ChoiceAmount : AmountRule
{
    public ChoiceAmount(IReadOnlyList<ChoiceAmountOption> options, string? notes = null)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (options.Count == 0)
        {
            throw new ArgumentException("Choice amount must contain at least one option.", nameof(options));
        }

        Options = options.ToArray();
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    public ChoiceAmount(IReadOnlyList<MoneyAmount> options, string? notes = null)
        : this(
            (options ?? throw new ArgumentNullException(nameof(options)))
                .Select(option => new ChoiceAmountOption(option))
                .ToArray(),
            notes)
    {
    }

    public IReadOnlyList<ChoiceAmountOption> Options { get; }

    public string? Notes { get; }
}

public sealed record PercentageSubsidy : AmountRule
{
    public PercentageSubsidy(decimal percentage, MoneyAmount? capAmount = null, string? notes = null)
    {
        if (percentage < 0m || percentage > 100m)
        {
            throw new ArgumentOutOfRangeException(nameof(percentage), "Percentage must be between 0 and 100.");
        }

        Percentage = percentage;
        CapAmount = capAmount;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    public decimal Percentage { get; }

    public MoneyAmount? CapAmount { get; }

    public string? Notes { get; }
}

public sealed record NaturalSupport : AmountRule
{
    public NaturalSupport(string description)
    {
        Description = DomainGuard.RequiredText(description, nameof(description));
    }

    public string Description { get; }
}

public sealed record StatusOnlyAmount : AmountRule
{
    public StatusOnlyAmount(string statusText)
    {
        StatusText = DomainGuard.RequiredText(statusText, nameof(statusText));
    }

    public string StatusText { get; }
}
