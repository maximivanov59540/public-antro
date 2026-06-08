using Antro.Application;
using Antro.Domain;

namespace Antro.Application.Tests;

public sealed class DeadlineEvaluatorTests
{
    private readonly DeadlineEvaluator evaluator = new();

    [Fact]
    public void Two_month_filing_deadline_is_active_before_due_date()
    {
        var result = Evaluate(
            new FilingDeadlineFromBirth(2, "Our Treasure style deadline."),
            today: new DateOnly(2026, 5, 22),
            childDate: new DateOnly(2026, 3, 23));

        Assert.Equal(DeadlineStatus.Urgent, result.Status);
        Assert.Equal(new DateOnly(2026, 5, 23), result.DeadlineDate);
        Assert.Equal(1, result.DaysLeft);
    }

    [Fact]
    public void Two_month_filing_deadline_is_active_on_due_date_using_inclusive_behavior()
    {
        var result = Evaluate(
            new FilingDeadlineFromBirth(2, "Our Treasure style deadline."),
            today: new DateOnly(2026, 5, 23),
            childDate: new DateOnly(2026, 3, 23));

        Assert.Equal(DeadlineStatus.Urgent, result.Status);
        Assert.Equal(new DateOnly(2026, 5, 23), result.DeadlineDate);
        Assert.Equal(0, result.DaysLeft);
    }

    [Fact]
    public void Two_month_filing_deadline_is_expired_after_due_date()
    {
        var result = Evaluate(
            new FilingDeadlineFromBirth(2, "Our Treasure style deadline."),
            today: new DateOnly(2026, 5, 24),
            childDate: new DateOnly(2026, 3, 23));

        Assert.Equal(DeadlineStatus.Expired, result.Status);
        Assert.Equal(new DateOnly(2026, 5, 23), result.DeadlineDate);
        Assert.Equal(-1, result.DaysLeft);
    }

    [Fact]
    public void Six_month_filing_deadline_is_calculated_from_birth()
    {
        var result = Evaluate(
            new FilingDeadlineFromBirth(6, "Federal birth grant style deadline."),
            today: new DateOnly(2026, 5, 23),
            childDate: new DateOnly(2025, 11, 23));

        Assert.Equal(DeadlineStatus.Urgent, result.Status);
        Assert.Equal(new DateOnly(2026, 5, 23), result.DeadlineDate);
    }

    [Fact]
    public void Twelve_month_filing_deadline_is_calculated_from_birth()
    {
        var result = Evaluate(
            new FilingDeadlineFromBirth(12, "Young family style deadline."),
            today: new DateOnly(2026, 5, 23),
            childDate: new DateOnly(2025, 5, 23));

        Assert.Equal(DeadlineStatus.Urgent, result.Status);
        Assert.Equal(new DateOnly(2026, 5, 23), result.DeadlineDate);
    }

    [Fact]
    public void Active_until_eighteen_months_is_active_through_boundary_date()
    {
        var result = Evaluate(
            new ActiveUntilChildAge(months: 18, years: null, notes: "Feeding breaks style boundary."),
            today: new DateOnly(2026, 5, 23),
            childDate: new DateOnly(2024, 11, 23));

        Assert.Equal(DeadlineStatus.Urgent, result.Status);
        Assert.Equal(new DateOnly(2026, 5, 23), result.DeadlineDate);
        Assert.Equal(DeadlineKind.ActiveUntil, result.Kind);
    }

    [Fact]
    public void Active_until_three_years_is_active_through_boundary_date()
    {
        var result = Evaluate(
            new ActiveUntilChildAge(months: null, years: 3, notes: "Three-year boundary."),
            today: new DateOnly(2026, 5, 23),
            childDate: new DateOnly(2023, 5, 23));

        Assert.Equal(DeadlineStatus.Urgent, result.Status);
        Assert.Equal(new DateOnly(2026, 5, 23), result.DeadlineDate);
    }

    [Fact]
    public void Fixed_policy_end_date_handles_before_on_and_after()
    {
        var before = Evaluate(new FixedPolicyEndDate(new DateOnly(2026, 5, 23), "Policy deadline."), today: new DateOnly(2026, 5, 22));
        var on = Evaluate(new FixedPolicyEndDate(new DateOnly(2026, 5, 23), "Policy deadline."), today: new DateOnly(2026, 5, 23));
        var after = Evaluate(new FixedPolicyEndDate(new DateOnly(2026, 5, 23), "Policy deadline."), today: new DateOnly(2026, 5, 24));

        Assert.Equal(DeadlineStatus.Urgent, before.Status);
        Assert.Equal(DeadlineStatus.Urgent, on.Status);
        Assert.Equal(DeadlineStatus.Expired, after.Status);
    }

    [Fact]
    public void Unknown_child_date_produces_unknown_when_required()
    {
        var context = CreateContext(new DateOnly(2026, 5, 23));
        var profile = new UserProfile();

        var result = evaluator.Evaluate(new FilingDeadlineFromBirth(6, "Needs birth date."), profile, context);

        Assert.Equal(DeadlineStatus.Unknown, result.Status);
        Assert.Null(result.DeadlineDate);
        Assert.Null(result.DaysLeft);
    }

    [Fact]
    public void No_deadline_produces_none()
    {
        var context = CreateContext(new DateOnly(2026, 5, 23));

        var result = evaluator.Evaluate(new NoDeadline("No deadline."), new UserProfile(), context);

        Assert.Equal(DeadlineStatus.None, result.Status);
        Assert.Equal(DeadlineKind.None, result.Kind);
        Assert.Null(result.DeadlineDate);
    }

    private DeadlineEvaluation Evaluate(
        DeadlineRule deadlineRule,
        DateOnly today,
        DateOnly? childDate = null)
    {
        var profile = childDate is null
            ? new UserProfile()
            : new UserProfile
            {
                ChildDate = childDate,
                DateInputKind = DateInputKind.BirthDate
            };

        return evaluator.Evaluate(deadlineRule, profile, CreateContext(today));
    }

    private static EvaluationContext CreateContext(DateOnly today) =>
        new(
            today,
            new RegionCode("RU-MOW"),
            new CatalogVersion("test-catalog"),
            new AssumedMvpEligibilityContext(true, true, true));
}
