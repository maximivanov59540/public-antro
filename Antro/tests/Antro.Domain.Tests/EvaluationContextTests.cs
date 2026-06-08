using Antro.Domain;

namespace Antro.Domain.Tests;

public sealed class EvaluationContextTests
{
    [Fact]
    public void EvaluationContext_carries_explicit_mvp_assumptions()
    {
        var assumptions = new AssumedMvpEligibilityContext(
            AssumesRussianCitizenship: true,
            AssumesMoscowRegistration: true,
            AssumesMoscow2026Rules: true);

        var context = new EvaluationContext(
            Today: new DateOnly(2026, 5, 23),
            Region: new RegionCode("moscow"),
            CatalogVersion: new CatalogVersion("2026.0"),
            Assumptions: assumptions);

        Assert.Equal(new DateOnly(2026, 5, 23), context.Today);
        Assert.Equal(new RegionCode("moscow"), context.Region);
        Assert.Equal(new CatalogVersion("2026.0"), context.CatalogVersion);
        Assert.True(context.Assumptions.AssumesRussianCitizenship);
        Assert.True(context.Assumptions.AssumesMoscowRegistration);
        Assert.True(context.Assumptions.AssumesMoscow2026Rules);
    }
}
