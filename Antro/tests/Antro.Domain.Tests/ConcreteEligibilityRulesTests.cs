using Antro.Domain;

namespace Antro.Domain.Tests;

public sealed class ConcreteEligibilityRulesTests
{
    private static readonly EvaluationContext DefaultContext = new(
        new DateOnly(2026, 5, 23),
        new RegionCode("RU-MOW"),
        new CatalogVersion("test-catalog"),
        new AssumedMvpEligibilityContext(true, true, true));

    [Fact]
    public void Income_band_rule_returns_unknown_for_missing_income()
    {
        var rule = new IncomeBandRule([IncomeBand.UpToOneLivingMinimum]);

        var result = rule.Evaluate(new UserProfile(), DefaultContext);

        Assert.Equal(RuleOutcome.Unknown, result.Outcome);
        Assert.Contains(MissingProfileField.IncomeBand, result.MissingFields);
        Assert.False(string.IsNullOrWhiteSpace(result.Explanation));
    }

    [Fact]
    public void Property_status_rule_can_request_review_without_false_denial()
    {
        var rule = new PropertyStatusRule(
            [PropertyStatus.DoesNotOwnHome],
            mismatchOutcome: RuleOutcome.NotApplicable);

        var result = rule.Evaluate(
            new UserProfile
            {
                PropertyStatus = PropertyStatus.OwnsHome
            },
            DefaultContext);

        Assert.Equal(RuleOutcome.NotApplicable, result.Outcome);
        Assert.False(string.IsNullOrWhiteSpace(result.Explanation));
    }

    [Fact]
    public void Assumed_moscow_registration_rule_fails_when_assumption_is_false()
    {
        var rule = new AssumedMoscowRegistrationRule();
        var context = new EvaluationContext(
            DefaultContext.Today,
            DefaultContext.Region,
            DefaultContext.CatalogVersion,
            new AssumedMvpEligibilityContext(true, false, true));

        var result = rule.Evaluate(new UserProfile(), context);

        Assert.Equal(RuleOutcome.Fail, result.Outcome);
        Assert.False(string.IsNullOrWhiteSpace(result.Explanation));
    }

    [Fact]
    public void Life_stage_rule_passes_for_expected_child_date()
    {
        var rule = new LifeStageRule([LifeStage.ExpectingChild]);
        var profile = new UserProfile
        {
            ChildDate = new DateOnly(2026, 6, 10),
            DateInputKind = DateInputKind.DueDate
        };

        var result = rule.Evaluate(profile, DefaultContext);

        Assert.Equal(RuleOutcome.Pass, result.Outcome);
    }

    [Fact]
    public void Child_age_rule_fails_after_upper_boundary()
    {
        var rule = new ChildAgeRule(maximumAgeInMonths: 18);
        var profile = new UserProfile
        {
            ChildDate = new DateOnly(2024, 11, 22),
            DateInputKind = DateInputKind.BirthDate
        };

        var result = rule.Evaluate(profile, DefaultContext);

        Assert.Equal(RuleOutcome.Fail, result.Outcome);
        Assert.False(string.IsNullOrWhiteSpace(result.Explanation));
    }
}
