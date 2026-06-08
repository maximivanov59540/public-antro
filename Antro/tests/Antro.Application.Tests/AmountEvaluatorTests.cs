using Antro.Application;
using Antro.Domain;

namespace Antro.Application.Tests;

public sealed class AmountEvaluatorTests
{
    private static readonly EvaluationContext Context = new(
        Today: new DateOnly(2026, 5, 23),
        Region: new RegionCode("RU-MOW"),
        CatalogVersion: new CatalogVersion("test-catalog"),
        Assumptions: new AssumedMvpEligibilityContext(true, true, true));

    private readonly IAmountEvaluator evaluator = new AmountEvaluator();

    [Fact]
    public void Fixed_one_time_rub_amount_is_exact()
    {
        var result = Evaluate(new FixedAmount(new MoneyAmount(20_000m, Currency.Rub, AmountPeriod.OneTime)));

        Assert.Equal(MoneyEstimateKind.Exact, result.Kind);
        Assert.Equal(new MoneyAmount(20_000m, Currency.Rub, AmountPeriod.OneTime), result.ExactAmount);
        Assert.True(result.CanParticipateInTotals);
        Assert.Equal("20 000 \u20BD", result.DisplayText);
    }

    [Fact]
    public void Monthly_up_to_amount_formats_deterministically()
    {
        var result = Evaluate(new UpToAmount(new MoneyAmount(83_021m, Currency.Rub, AmountPeriod.Monthly)));

        Assert.Equal(MoneyEstimateKind.UpTo, result.Kind);
        Assert.False(result.CanParticipateInTotals);
        Assert.Equal("\u0434\u043e 83 021 \u20BD/\u043c\u0435\u0441", result.DisplayText);
    }

    [Fact]
    public void Amount_range_formats_deterministically()
    {
        var result = Evaluate(
            new AmountRange(
                new MoneyAmount(124_702m, Currency.Rub, AmountPeriod.OneTime),
                new MoneyAmount(955_836m, Currency.Rub, AmountPeriod.OneTime)));

        Assert.Equal(MoneyEstimateKind.Range, result.Kind);
        Assert.False(result.CanParticipateInTotals);
        Assert.Equal("124 702\u2013955 836 \u20BD", result.DisplayText);
    }

    [Fact]
    public void Choice_amount_selects_first_child_amount_when_profile_is_known()
    {
        var result = Evaluate(
            CreateChildOrderChoiceAmount(),
            new UserProfile
            {
                ChildOrder = ChildOrder.First
            });

        Assert.Equal(MoneyEstimateKind.Choice, result.Kind);
        Assert.Equal(new MoneyAmount(100_000m, Currency.Rub, AmountPeriod.OneTime), result.ExactAmount);
        Assert.True(result.CanParticipateInTotals);
        Assert.Equal("100 000 \u20BD", result.DisplayText);
    }

    [Fact]
    public void Choice_amount_selects_second_child_amount_when_profile_is_known()
    {
        var result = Evaluate(
            CreateChildOrderChoiceAmount(),
            new UserProfile
            {
                ChildOrder = ChildOrder.Second
            });

        Assert.Equal(MoneyEstimateKind.Choice, result.Kind);
        Assert.Equal(new MoneyAmount(200_000m, Currency.Rub, AmountPeriod.OneTime), result.ExactAmount);
        Assert.True(result.CanParticipateInTotals);
        Assert.Equal("200 000 \u20BD", result.DisplayText);
    }

    [Fact]
    public void Choice_amount_preserves_multiple_options_when_required_profile_field_is_unknown()
    {
        var result = Evaluate(CreateChildOrderChoiceAmount(), new UserProfile());

        Assert.Equal(MoneyEstimateKind.Choice, result.Kind);
        Assert.Null(result.ExactAmount);
        Assert.False(result.CanParticipateInTotals);
        Assert.Equal("100 000\u2013200 000 \u20BD", result.DisplayText);
        Assert.Equal([MissingProfileField.ChildOrder], result.MissingFields);
        Assert.Equal(2, result.Options.Count);
    }

    [Fact]
    public void Natural_support_produces_non_monetary_estimate()
    {
        var result = Evaluate(new NaturalSupport("Milk kitchen products."));

        Assert.Equal(MoneyEstimateKind.NaturalSupport, result.Kind);
        Assert.False(result.IsMonetary);
        Assert.False(result.CanParticipateInTotals);
        Assert.Equal("\u041d\u0430\u0442\u0443\u0440\u0430\u043b\u044c\u043d\u0430\u044f \u043f\u043e\u043c\u043e\u0449\u044c", result.DisplayText);
    }

    [Fact]
    public void Status_only_amount_does_not_produce_money()
    {
        var result = Evaluate(new StatusOnlyAmount("\u0422\u0440\u0443\u0434\u043e\u0432\u0430\u044f \u0433\u0430\u0440\u0430\u043d\u0442\u0438\u044f"));

        Assert.Equal(MoneyEstimateKind.StatusOnly, result.Kind);
        Assert.False(result.IsMonetary);
        Assert.False(result.CanParticipateInTotals);
        Assert.Null(result.ExactAmount);
        Assert.Equal("\u0422\u0440\u0443\u0434\u043e\u0432\u0430\u044f \u0433\u0430\u0440\u0430\u043d\u0442\u0438\u044f", result.DisplayText);
    }

    [Fact]
    public void Percentage_subsidy_does_not_become_fake_fixed_ruble_amount()
    {
        var result = Evaluate(new PercentageSubsidy(35m));

        Assert.Equal(MoneyEstimateKind.Percentage, result.Kind);
        Assert.Equal(35m, result.Percentage);
        Assert.False(result.CanParticipateInTotals);
        Assert.Null(result.ExactAmount);
        Assert.Equal("35%", result.DisplayText);
    }

    private MoneyEstimate Evaluate(AmountRule amountRule, UserProfile? profile = null) =>
        evaluator.Evaluate(amountRule, profile ?? new UserProfile(), Context);

    private static ChoiceAmount CreateChildOrderChoiceAmount() =>
        new(
            [
                new ChoiceAmountOption(
                    new MoneyAmount(100_000m, Currency.Rub, AmountPeriod.OneTime),
                    label: "First child",
                    childOrder: ChildOrder.First),
                new ChoiceAmountOption(
                    new MoneyAmount(200_000m, Currency.Rub, AmountPeriod.OneTime),
                    label: "Second child",
                    childOrder: ChildOrder.Second)
            ],
            "Amount depends on child order.");
}
