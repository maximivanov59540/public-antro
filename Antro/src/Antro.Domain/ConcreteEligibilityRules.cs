namespace Antro.Domain;

public abstract record EligibilityRuleBase : IEligibilityRule
{
    protected EligibilityRuleBase(string code, string description)
    {
        Code = DomainGuard.RequiredText(code, nameof(code));
        Description = DomainGuard.RequiredText(description, nameof(description));
    }

    public string Code { get; }

    public string Description { get; }

    public abstract EligibilityCheckResult Evaluate(UserProfile profile, EvaluationContext context);

    protected EligibilityCheckResult Pass(string? explanation = null) =>
        new(Code, RuleOutcome.Pass, explanation: explanation);

    protected EligibilityCheckResult Fail(string explanation) =>
        new(Code, RuleOutcome.Fail, explanation: explanation);

    protected EligibilityCheckResult Unknown(MissingProfileField missingField, string explanation) =>
        new(Code, RuleOutcome.Unknown, [missingField], explanation);

    protected EligibilityCheckResult Unknown(IReadOnlyList<MissingProfileField> missingFields, string explanation) =>
        new(Code, RuleOutcome.Unknown, missingFields, explanation);

    protected EligibilityCheckResult NotApplicable(string? explanation = null) =>
        new(Code, RuleOutcome.NotApplicable, explanation: explanation);
}

public sealed record ChildAgeRule : EligibilityRuleBase
{
    public ChildAgeRule(
        int? minimumAgeInMonths = null,
        int? maximumAgeInMonths = null,
        string? tooYoungExplanation = null,
        string? tooOldExplanation = null,
        string? unknownExplanation = null)
        : base("child-age", "Checks whether the child's current age is within the supported range.")
    {
        if (minimumAgeInMonths is null && maximumAgeInMonths is null)
        {
            throw new ArgumentException("At least one age boundary must be provided.");
        }

        if (minimumAgeInMonths < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minimumAgeInMonths));
        }

        if (maximumAgeInMonths < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumAgeInMonths));
        }

        if (minimumAgeInMonths is not null && maximumAgeInMonths is not null && minimumAgeInMonths > maximumAgeInMonths)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumAgeInMonths), "Maximum age cannot be less than minimum age.");
        }

        MinimumAgeInMonths = minimumAgeInMonths;
        MaximumAgeInMonths = maximumAgeInMonths;
        TooYoungExplanation = tooYoungExplanation ?? "Эта мера становится актуальной позже по возрасту ребёнка.";
        TooOldExplanation = tooOldExplanation ?? "Сейчас эта мера уже не подходит по возрасту ребёнка.";
        UnknownExplanation = unknownExplanation ?? "Для этой меры нужна точная дата рождения ребёнка.";
    }

    public int? MinimumAgeInMonths { get; }

    public int? MaximumAgeInMonths { get; }

    public string TooYoungExplanation { get; }

    public string TooOldExplanation { get; }

    public string UnknownExplanation { get; }

    public override EligibilityCheckResult Evaluate(UserProfile profile, EvaluationContext context)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(context);

        if (TryGetBirthDate(profile, out var birthDate, out var missingFields) is false)
        {
            return Unknown(missingFields, UnknownExplanation);
        }

        if (MinimumAgeInMonths is not null && context.Today < birthDate.AddMonths(MinimumAgeInMonths.Value))
        {
            return Fail(TooYoungExplanation);
        }

        if (MaximumAgeInMonths is not null && context.Today > birthDate.AddMonths(MaximumAgeInMonths.Value))
        {
            return Fail(TooOldExplanation);
        }

        return Pass();
    }

    private static bool TryGetBirthDate(UserProfile profile, out DateOnly birthDate, out MissingProfileField[] missingFields)
    {
        var fields = new List<MissingProfileField>();

        if (profile.ChildDate is null)
        {
            fields.Add(MissingProfileField.ChildDate);
        }

        if (profile.DateInputKind != DateInputKind.BirthDate)
        {
            fields.Add(MissingProfileField.DateInputKind);
        }

        if (fields.Count > 0)
        {
            birthDate = default;
            missingFields = fields.Distinct().ToArray();
            return false;
        }

        birthDate = profile.ChildDate!.Value;
        missingFields = Array.Empty<MissingProfileField>();
        return true;
    }
}

public sealed record ChildOrderRule : EligibilityRuleBase
{
    private readonly HashSet<ChildOrder> allowedOrders;

    public ChildOrderRule(
        IReadOnlyList<ChildOrder> allowedOrders,
        string? failExplanation = null,
        string? unknownExplanation = null)
        : base("child-order", "Checks whether the child's order is within the supported set.")
    {
        ArgumentNullException.ThrowIfNull(allowedOrders);

        if (allowedOrders.Count == 0)
        {
            throw new ArgumentException("At least one child order must be allowed.", nameof(allowedOrders));
        }

        this.allowedOrders = allowedOrders.ToHashSet();
        FailExplanation = failExplanation ?? "Сейчас эта мера не подходит по очередности рождения ребёнка.";
        UnknownExplanation = unknownExplanation ?? "Для этой меры нужно уточнить, какой по счёту это ребёнок.";
    }

    public string FailExplanation { get; }

    public string UnknownExplanation { get; }

    public override EligibilityCheckResult Evaluate(UserProfile profile, EvaluationContext context)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(context);

        if (profile.ChildOrder is null)
        {
            return Unknown(MissingProfileField.ChildOrder, UnknownExplanation);
        }

        return allowedOrders.Contains(profile.ChildOrder.Value)
            ? Pass()
            : Fail(FailExplanation);
    }
}

public sealed record IncomeBandRule : EligibilityRuleBase
{
    private readonly HashSet<IncomeBand> allowedBands;

    public IncomeBandRule(
        IReadOnlyList<IncomeBand> allowedBands,
        string? failExplanation = null,
        string? unknownExplanation = null)
        : base("income-band", "Checks whether the family's income band is within the supported set.")
    {
        ArgumentNullException.ThrowIfNull(allowedBands);

        if (allowedBands.Count == 0)
        {
            throw new ArgumentException("At least one income band must be allowed.", nameof(allowedBands));
        }

        this.allowedBands = allowedBands
            .Where(band => band != IncomeBand.Unknown)
            .ToHashSet();

        if (this.allowedBands.Count == 0)
        {
            throw new ArgumentException("Unknown income cannot be the only allowed value.", nameof(allowedBands));
        }

        FailExplanation = failExplanation ?? "Сейчас эта мера не подходит по выбранному доходному диапазону.";
        UnknownExplanation = unknownExplanation ?? "Эта выплата зависит от дохода семьи. Уточните доходный диапазон.";
    }

    public string FailExplanation { get; }

    public string UnknownExplanation { get; }

    public override EligibilityCheckResult Evaluate(UserProfile profile, EvaluationContext context)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(context);

        if (profile.IncomeBand is null || profile.IncomeBand == Domain.IncomeBand.Unknown)
        {
            return Unknown(MissingProfileField.IncomeBand, UnknownExplanation);
        }

        return allowedBands.Contains(profile.IncomeBand.Value)
            ? Pass()
            : Fail(FailExplanation);
    }
}

public sealed record PropertyStatusRule : EligibilityRuleBase
{
    private readonly HashSet<PropertyStatus> allowedStatuses;

    public PropertyStatusRule(
        IReadOnlyList<PropertyStatus> allowedStatuses,
        RuleOutcome mismatchOutcome = RuleOutcome.Fail,
        string? failExplanation = null,
        string? unknownExplanation = null,
        string? notApplicableExplanation = null)
        : base("property-status", "Checks whether the family's property status fits the supported condition.")
    {
        ArgumentNullException.ThrowIfNull(allowedStatuses);

        if (allowedStatuses.Count == 0)
        {
            throw new ArgumentException("At least one property status must be allowed.", nameof(allowedStatuses));
        }

        if (mismatchOutcome is not (RuleOutcome.Fail or RuleOutcome.NotApplicable))
        {
            throw new ArgumentOutOfRangeException(nameof(mismatchOutcome), "Property mismatches must resolve to Fail or NotApplicable.");
        }

        this.allowedStatuses = allowedStatuses.ToHashSet();
        MismatchOutcome = mismatchOutcome;
        FailExplanation = failExplanation ?? "Сейчас эта мера не подходит по выбранному жилищному статусу.";
        UnknownExplanation = unknownExplanation ?? "Для этой меры нужно уточнить жилищный статус семьи.";
        NotApplicableExplanation = notApplicableExplanation ?? "Для этой меры потребуется дополнительная проверка жилищного статуса.";
    }

    public RuleOutcome MismatchOutcome { get; }

    public string FailExplanation { get; }

    public string UnknownExplanation { get; }

    public string NotApplicableExplanation { get; }

    public override EligibilityCheckResult Evaluate(UserProfile profile, EvaluationContext context)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(context);

        if (profile.PropertyStatus is null)
        {
            return Unknown(MissingProfileField.PropertyStatus, UnknownExplanation);
        }

        if (allowedStatuses.Contains(profile.PropertyStatus.Value))
        {
            return Pass();
        }

        return MismatchOutcome == RuleOutcome.NotApplicable
            ? NotApplicable(NotApplicableExplanation)
            : Fail(FailExplanation);
    }
}

public sealed record ParentAgeBandRule : EligibilityRuleBase
{
    private readonly HashSet<ParentAgeBand> allowedBands;

    public ParentAgeBandRule(
        IReadOnlyList<ParentAgeBand> allowedBands,
        string? failExplanation = null,
        string? unknownExplanation = null)
        : base("parent-age-band", "Checks whether the parent age band fits the supported condition.")
    {
        ArgumentNullException.ThrowIfNull(allowedBands);

        if (allowedBands.Count == 0)
        {
            throw new ArgumentException("At least one parent age band must be allowed.", nameof(allowedBands));
        }

        this.allowedBands = allowedBands.ToHashSet();
        FailExplanation = failExplanation ?? "Эта мера доступна молодым родителям при соблюдении возрастного условия.";
        UnknownExplanation = unknownExplanation ?? "Для этой меры нужно уточнить возрастную категорию родителей.";
    }

    public string FailExplanation { get; }

    public string UnknownExplanation { get; }

    public override EligibilityCheckResult Evaluate(UserProfile profile, EvaluationContext context)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(context);

        if (profile.ParentAgeBand is null)
        {
            return Unknown(MissingProfileField.ParentAgeBand, UnknownExplanation);
        }

        return allowedBands.Contains(profile.ParentAgeBand.Value)
            ? Pass()
            : Fail(FailExplanation);
    }
}

public sealed record EmploymentStatusRule : EligibilityRuleBase
{
    private readonly HashSet<EmploymentStatus> allowedStatuses;

    public EmploymentStatusRule(
        IReadOnlyList<EmploymentStatus> allowedStatuses,
        string audience = "родителя",
        string? failExplanation = null,
        string? unknownExplanation = null)
        : base("employment-status", "Checks whether the employment status fits the supported condition.")
    {
        ArgumentNullException.ThrowIfNull(allowedStatuses);

        if (allowedStatuses.Count == 0)
        {
            throw new ArgumentException("At least one employment status must be allowed.", nameof(allowedStatuses));
        }

        this.allowedStatuses = allowedStatuses.ToHashSet();
        Audience = DomainGuard.RequiredText(audience, nameof(audience));
        FailExplanation = failExplanation ?? $"Для этой меры нужен подходящий трудовой статус {Audience}.";
        UnknownExplanation = unknownExplanation ?? $"Для этой меры нужно уточнить трудовой статус {Audience}.";
    }

    public string Audience { get; }

    public string FailExplanation { get; }

    public string UnknownExplanation { get; }

    public override EligibilityCheckResult Evaluate(UserProfile profile, EvaluationContext context)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(context);

        if (profile.EmploymentStatus is null)
        {
            return Unknown(MissingProfileField.EmploymentStatus, UnknownExplanation);
        }

        return allowedStatuses.Contains(profile.EmploymentStatus.Value)
            ? Pass()
            : Fail(FailExplanation);
    }
}

public sealed record MatCapitalHistoryRule : EligibilityRuleBase
{
    private readonly HashSet<MatCapitalHistory> allowedHistories;

    public MatCapitalHistoryRule(
        IReadOnlyList<MatCapitalHistory> allowedHistories,
        string? failExplanation = null,
        string? unknownExplanation = null)
        : base("matcapital-history", "Checks whether the family's matcapital history fits the supported condition.")
    {
        ArgumentNullException.ThrowIfNull(allowedHistories);

        if (allowedHistories.Count == 0)
        {
            throw new ArgumentException("At least one matcapital history value must be allowed.", nameof(allowedHistories));
        }

        this.allowedHistories = allowedHistories.ToHashSet();
        FailExplanation = failExplanation ?? "Сейчас эта мера не подходит по истории использования маткапитала.";
        UnknownExplanation = unknownExplanation ?? "Для этой меры нужно уточнить историю использования маткапитала.";
    }

    public string FailExplanation { get; }

    public string UnknownExplanation { get; }

    public override EligibilityCheckResult Evaluate(UserProfile profile, EvaluationContext context)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(context);

        if (profile.MatCapitalHistory is null)
        {
            return Unknown(MissingProfileField.MatCapitalHistory, UnknownExplanation);
        }

        return allowedHistories.Contains(profile.MatCapitalHistory.Value)
            ? Pass()
            : Fail(FailExplanation);
    }
}

public sealed record MortgageIntentRule : EligibilityRuleBase
{
    private readonly HashSet<MortgageIntent> allowedIntents;

    public MortgageIntentRule(
        IReadOnlyList<MortgageIntent> allowedIntents,
        RuleOutcome mismatchOutcome = RuleOutcome.Fail,
        string? failExplanation = null,
        string? unknownExplanation = null,
        string? notApplicableExplanation = null)
        : base("mortgage-intent", "Checks whether the mortgage intent matches the supported route.")
    {
        ArgumentNullException.ThrowIfNull(allowedIntents);

        if (allowedIntents.Count == 0)
        {
            throw new ArgumentException("At least one mortgage intent value must be allowed.", nameof(allowedIntents));
        }

        if (mismatchOutcome is not (RuleOutcome.Fail or RuleOutcome.NotApplicable))
        {
            throw new ArgumentOutOfRangeException(nameof(mismatchOutcome), "Mortgage mismatches must resolve to Fail or NotApplicable.");
        }

        this.allowedIntents = allowedIntents.ToHashSet();
        MismatchOutcome = mismatchOutcome;
        FailExplanation = failExplanation ?? "Эта мера обычно актуальна, когда семья планирует ипотеку или уже оформляет её.";
        UnknownExplanation = unknownExplanation ?? "Для этой меры нужно уточнить планы семьи по ипотеке.";
        NotApplicableExplanation = notApplicableExplanation ?? "Эта мера может быть неактуальна без ипотечного маршрута.";
    }

    public RuleOutcome MismatchOutcome { get; }

    public string FailExplanation { get; }

    public string UnknownExplanation { get; }

    public string NotApplicableExplanation { get; }

    public override EligibilityCheckResult Evaluate(UserProfile profile, EvaluationContext context)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(context);

        if (profile.MortgageIntent is null)
        {
            return Unknown(MissingProfileField.MortgageIntent, UnknownExplanation);
        }

        if (allowedIntents.Contains(profile.MortgageIntent.Value))
        {
            return Pass();
        }

        return MismatchOutcome == RuleOutcome.NotApplicable
            ? NotApplicable(NotApplicableExplanation)
            : Fail(FailExplanation);
    }
}

public sealed record AssumedMoscowRegistrationRule : EligibilityRuleBase
{
    public AssumedMoscowRegistrationRule(string? failExplanation = null)
        : base("assumed-moscow-registration", "Checks the MVP assumption about Moscow registration.")
    {
        FailExplanation = failExplanation ?? "В MVP эта мера показывается при предположении о московской регистрации.";
    }

    public string FailExplanation { get; }

    public override EligibilityCheckResult Evaluate(UserProfile profile, EvaluationContext context)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(context);

        return context.Assumptions.AssumesMoscowRegistration
            ? Pass()
            : Fail(FailExplanation);
    }
}

public sealed record AssumedRussianCitizenshipRule : EligibilityRuleBase
{
    public AssumedRussianCitizenshipRule(string? failExplanation = null)
        : base("assumed-russian-citizenship", "Checks the MVP assumption about Russian citizenship.")
    {
        FailExplanation = failExplanation ?? "В MVP эта мера показывается при предположении о российском гражданстве.";
    }

    public string FailExplanation { get; }

    public override EligibilityCheckResult Evaluate(UserProfile profile, EvaluationContext context)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(context);

        return context.Assumptions.AssumesRussianCitizenship
            ? Pass()
            : Fail(FailExplanation);
    }
}

public sealed record LifeStageRule : EligibilityRuleBase
{
    private readonly HashSet<LifeStage> allowedStages;

    public LifeStageRule(
        IReadOnlyList<LifeStage> allowedStages,
        string? failExplanation = null,
        string? unknownExplanation = null)
        : base("life-stage", "Checks whether the benefit is intended for the current life stage.")
    {
        ArgumentNullException.ThrowIfNull(allowedStages);

        if (allowedStages.Count == 0)
        {
            throw new ArgumentException("At least one life stage must be allowed.", nameof(allowedStages));
        }

        this.allowedStages = allowedStages.ToHashSet();
        FailExplanation = failExplanation ?? "Сейчас эта мера не относится к выбранному жизненному этапу семьи.";
        UnknownExplanation = unknownExplanation ?? "Для этой меры нужно уточнить дату рождения ребёнка или срок беременности.";
    }

    public string FailExplanation { get; }

    public string UnknownExplanation { get; }

    public override EligibilityCheckResult Evaluate(UserProfile profile, EvaluationContext context)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(context);

        if (profile.ChildDate is null)
        {
            return Unknown(MissingProfileField.ChildDate, UnknownExplanation);
        }

        if (profile.DateInputKind == DateInputKind.Unknown)
        {
            return Unknown(MissingProfileField.DateInputKind, UnknownExplanation);
        }

        LifeStage currentStage;

        try
        {
            currentStage = ChildLifeStageCalculator.Classify(profile.ChildDate.Value, profile.DateInputKind, context.Today);
        }
        catch (ArgumentException)
        {
            return Unknown([MissingProfileField.ChildDate, MissingProfileField.DateInputKind], UnknownExplanation);
        }

        return allowedStages.Contains(currentStage)
            ? Pass()
            : Fail(FailExplanation);
    }
}
