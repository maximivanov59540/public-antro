using Antro.Domain;

namespace Antro.Domain.Tests;

public sealed class ChildLifeStageCalculatorTests
{
    private static readonly DateOnly Today = new(2026, 5, 23);

    [Theory]
    [InlineData(2026, 5, 23, LifeStage.NewbornZeroToTwoMonths)]
    [InlineData(2026, 3, 23, LifeStage.UpToSixMonths)]
    [InlineData(2025, 11, 23, LifeStage.UpToEighteenMonths)]
    [InlineData(2024, 11, 23, LifeStage.UpToThreeYears)]
    [InlineData(2023, 5, 23, LifeStage.UpToThreeYears)]
    [InlineData(2023, 5, 22, LifeStage.OlderThanThreeYears)]
    public void ClassifyBirthDate_returns_expected_life_stage_at_boundaries(
        int year,
        int month,
        int day,
        LifeStage expected)
    {
        var birthDate = new DateOnly(year, month, day);

        var result = ChildLifeStageCalculator.ClassifyBirthDate(birthDate, Today);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Classify_due_date_returns_expecting_child()
    {
        var dueDate = new DateOnly(2026, 6, 1);

        var result = ChildLifeStageCalculator.Classify(dueDate, DateInputKind.DueDate, Today);

        Assert.Equal(LifeStage.ExpectingChild, result);
    }
}
