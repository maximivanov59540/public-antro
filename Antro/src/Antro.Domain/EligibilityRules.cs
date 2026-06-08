namespace Antro.Domain;

public enum RuleOutcome
{
    Pass = 0,
    Fail = 1,
    Unknown = 2,
    NotApplicable = 3
}

public enum MissingProfileField
{
    ChildDate = 0,
    DateInputKind = 1,
    ChildOrder = 2,
    MatCapitalHistory = 3,
    FamilyStatus = 4,
    EmploymentStatus = 5,
    ParentAgeBand = 6,
    IncomeBand = 7,
    PropertyStatus = 8,
    MortgageIntent = 9
}

public sealed record EligibilityCheckResult
{
    public EligibilityCheckResult(
        string ruleCode,
        RuleOutcome outcome,
        IReadOnlyList<MissingProfileField>? missingFields = null,
        string? explanation = null)
    {
        RuleCode = DomainGuard.RequiredText(ruleCode, nameof(ruleCode));
        Outcome = outcome;
        MissingFields = missingFields?.ToArray() ?? Array.Empty<MissingProfileField>();
        Explanation = string.IsNullOrWhiteSpace(explanation) ? null : explanation.Trim();
    }

    public string RuleCode { get; }

    public RuleOutcome Outcome { get; }

    public IReadOnlyList<MissingProfileField> MissingFields { get; }

    public string? Explanation { get; }
}

public interface IEligibilityRule
{
    string Code { get; }

    string Description { get; }

    EligibilityCheckResult Evaluate(UserProfile profile, EvaluationContext context);
}
