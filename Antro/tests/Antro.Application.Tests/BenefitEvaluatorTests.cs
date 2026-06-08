using Antro.Application;
using Antro.Domain;

namespace Antro.Application.Tests;

public sealed class BenefitEvaluatorTests
{
    private static readonly EvaluationContext Context = new(
        Today: new DateOnly(2026, 5, 23),
        Region: new RegionCode("RU-MOW"),
        CatalogVersion: new CatalogVersion("test-catalog"),
        Assumptions: new AssumedMvpEligibilityContext(true, true, true));

    private readonly IBenefitEvaluator evaluator = new BenefitEvaluator();

    [Fact]
    public void All_pass_rules_produce_available()
    {
        var benefit = CreateBenefit(
            type: BenefitType.IncomeDependentPayment,
            eligibilityRules:
            [
                new StubEligibilityRule("pass-1", RuleOutcome.Pass, "Passed."),
                new StubEligibilityRule("pass-2", RuleOutcome.NotApplicable, "Not applicable.")
            ]);

        var result = evaluator.Evaluate([benefit], new UserProfile(), Context).Single();

        Assert.Equal(AvailabilityStatus.Available, result.AvailabilityStatus);
        Assert.Equal(DeadlineStatus.None, result.DeadlineStatus);
        Assert.Equal(DeadlineKind.None, result.Deadline.Kind);
        Assert.NotNull(result.MoneyEstimate);
        Assert.Equal(MoneyEstimateKind.Exact, result.MoneyEstimate!.Kind);
        Assert.Equal("10 000 \u20BD/\u043c\u0435\u0441", result.MoneyEstimate.DisplayText);
        Assert.Empty(result.MissingFields);
    }

    [Fact]
    public void One_fail_rule_produces_unavailable_with_reason()
    {
        var benefit = CreateBenefit(
            eligibilityRules:
            [
                new StubEligibilityRule("fail-1", RuleOutcome.Fail, "Income is above the program limit.")
            ]);

        var result = evaluator.Evaluate([benefit], new UserProfile(), Context).Single();

        Assert.Equal(AvailabilityStatus.Unavailable, result.AvailabilityStatus);
        Assert.Equal("Income is above the program limit.", result.UserFacingReason);
    }

    [Fact]
    public void Unknown_rule_produces_needs_more_info_with_missing_field()
    {
        var benefit = CreateBenefit(
            eligibilityRules:
            [
                new StubEligibilityRule(
                    "unknown-1",
                    RuleOutcome.Unknown,
                    "Child order is required.",
                    [MissingProfileField.ChildOrder])
            ]);

        var result = evaluator.Evaluate([benefit], new UserProfile(), Context).Single();

        Assert.Equal(AvailabilityStatus.NeedsMoreInfo, result.AvailabilityStatus);
        Assert.Equal("Child order is required.", result.UserFacingReason);
        Assert.Contains(MissingProfileField.ChildOrder, result.MissingFields);
    }

    [Fact]
    public void Automatic_benefit_can_become_automatic()
    {
        var benefit = CreateBenefit(type: BenefitType.Automatic);

        var result = evaluator.Evaluate([benefit], new UserProfile(), Context).Single();

        Assert.Equal(AvailabilityStatus.Automatic, result.AvailabilityStatus);
        Assert.Contains(result.Badges, badge => badge.SemanticKey == "automatic");
    }

    [Fact]
    public void Informational_benefit_can_become_informational()
    {
        var benefit = CreateBenefit(type: BenefitType.InformationalRight);

        var result = evaluator.Evaluate([benefit], new UserProfile(), Context).Single();

        Assert.Equal(AvailabilityStatus.Informational, result.AvailabilityStatus);
        Assert.Contains(result.Badges, badge => badge.SemanticKey == "informational");
    }

    [Fact]
    public void Expired_deadline_produces_expired_availability()
    {
        var benefit = CreateBenefit(deadlineRule: new FilingDeadlineFromBirth(2, "Deadline applies after birth."));
        var profile = new UserProfile
        {
            ChildDate = new DateOnly(2026, 3, 22),
            DateInputKind = DateInputKind.BirthDate
        };

        var result = evaluator.Evaluate([benefit], profile, Context).Single();

        Assert.Equal(AvailabilityStatus.Expired, result.AvailabilityStatus);
        Assert.Equal(DeadlineStatus.Expired, result.DeadlineStatus);
        Assert.Equal(DeadlineKind.FilingDeadline, result.Deadline.Kind);
    }

    [Fact]
    public void Stage_based_expiry_does_not_invent_a_deadline()
    {
        var benefit = CreateBenefit(
            stageAvailability:
            [
                new LifeStageAvailability(
                    LifeStage.NewbornZeroToTwoMonths,
                    isVisible: true,
                    status: StageAvailabilityStatus.Expired,
                    explanation: "Shown as expired in this stage.")
            ]);
        var profile = new UserProfile
        {
            ChildDate = new DateOnly(2026, 5, 1),
            DateInputKind = DateInputKind.BirthDate
        };

        var result = evaluator.Evaluate([benefit], profile, Context).Single();

        Assert.Equal(AvailabilityStatus.Expired, result.AvailabilityStatus);
        Assert.Equal(DeadlineStatus.None, result.DeadlineStatus);
        Assert.Equal(DeadlineKind.None, result.Deadline.Kind);
    }

    [Fact]
    public void Amount_choice_missing_field_is_carried_into_evaluated_benefit()
    {
        var benefit = CreateBenefit(
            amountRule: new ChoiceAmount(
                [
                    new ChoiceAmountOption(
                        new MoneyAmount(100_000m, Currency.Rub, AmountPeriod.OneTime),
                        childOrder: ChildOrder.First),
                    new ChoiceAmountOption(
                        new MoneyAmount(200_000m, Currency.Rub, AmountPeriod.OneTime),
                        childOrder: ChildOrder.Second)
                ],
                "Amount depends on child order."));

        var result = evaluator.Evaluate([benefit], new UserProfile(), Context).Single();

        Assert.Equal(AvailabilityStatus.Available, result.AvailabilityStatus);
        Assert.Contains(MissingProfileField.ChildOrder, result.MissingFields);
        Assert.NotNull(result.MoneyEstimate);
        Assert.Equal(MoneyEstimateKind.Choice, result.MoneyEstimate!.Kind);
        Assert.Equal("100 000\u2013200 000 \u20BD", result.MoneyEstimate.DisplayText);
    }

    [Fact]
    public void Evaluator_processes_empty_profile_deterministically()
    {
        var benefit = CreateBenefit(
            deadlineRule: new FilingDeadlineFromBirth(6, "Requires birth date."),
            eligibilityRules:
            [
                new StubEligibilityRule(
                    "unknown-date",
                    RuleOutcome.Unknown,
                    "Birth date is required.",
                    [MissingProfileField.ChildDate, MissingProfileField.DateInputKind])
            ]);

        var result = evaluator.Evaluate([benefit], new UserProfile(), Context).Single();

        Assert.Equal(AvailabilityStatus.NeedsMoreInfo, result.AvailabilityStatus);
        Assert.Equal(DeadlineStatus.Unknown, result.DeadlineStatus);
        Assert.Equal(
            [MissingProfileField.ChildDate, MissingProfileField.DateInputKind],
            result.MissingFields.OrderBy(field => field).ToArray());
    }

    [Fact]
    public void Evaluator_does_not_require_ui()
    {
        var referencedAssemblies = typeof(BenefitEvaluator)
            .Assembly
            .GetReferencedAssemblies()
            .Select(assemblyName => assemblyName.Name)
            .ToArray();

        Assert.DoesNotContain("Antro.Web", referencedAssemblies);
        Assert.DoesNotContain(referencedAssemblies, name => name?.Contains("Blazor", StringComparison.OrdinalIgnoreCase) == true);
    }

    private static Benefit CreateBenefit(
        BenefitType type = BenefitType.IncomeDependentPayment,
        AmountRule? amountRule = null,
        DeadlineRule? deadlineRule = null,
        IReadOnlyList<IEligibilityRule>? eligibilityRules = null,
        IReadOnlyList<LifeStageAvailability>? stageAvailability = null)
    {
        return new Benefit(
            id: new BenefitId(Guid.NewGuid().ToString("N")),
            slug: Guid.NewGuid().ToString("N"),
            region: new RegionCode("RU-MOW"),
            catalogVersion: new CatalogVersion("test-catalog"),
            type: type,
            tier: BenefitTier.Tier1Core,
            category: BenefitCategory.FamilyIncomeSupport,
            copy: new BenefitCopy(
                "Test benefit",
                null,
                "Short description",
                "Detailed description",
                null),
            amountRule: amountRule ?? new FixedAmount(new MoneyAmount(10_000m, Currency.Rub, AmountPeriod.Monthly)),
            deadlineRule: deadlineRule ?? new NoDeadline("No ordinary deadline."),
            stageAvailability: stageAvailability ??
            [
                new LifeStageAvailability(
                    LifeStage.NewbornZeroToTwoMonths,
                    isVisible: true,
                    status: StageAvailabilityStatus.Active,
                    explanation: "Visible in the current stage.")
            ],
            eligibilityRules: eligibilityRules ?? Array.Empty<IEligibilityRule>(),
            documents:
            [
                new DocumentRequirement("Passport", "Draft document", null)
            ],
            actionSteps:
            [
                new ActionStep(1, "Submit", "Draft step", null)
            ],
            legalSources:
            [
                new LegalSource(
                    "Test source",
                    "TEST-1",
                    null,
                    LegalSourceLevel.Other,
                    null,
                    VerificationStatus.Draft,
                    "Draft source")
            ]);
    }

    private sealed class StubEligibilityRule : IEligibilityRule
    {
        private readonly RuleOutcome outcome;
        private readonly string? explanation;
        private readonly IReadOnlyList<MissingProfileField> missingFields;

        public StubEligibilityRule(
            string code,
            RuleOutcome outcome,
            string? explanation = null,
            IReadOnlyList<MissingProfileField>? missingFields = null)
        {
            Code = code;
            Description = code;
            this.outcome = outcome;
            this.explanation = explanation;
            this.missingFields = missingFields ?? Array.Empty<MissingProfileField>();
        }

        public string Code { get; }

        public string Description { get; }

        public EligibilityCheckResult Evaluate(UserProfile profile, EvaluationContext context) =>
            new(Code, outcome, missingFields, explanation);
    }
}
