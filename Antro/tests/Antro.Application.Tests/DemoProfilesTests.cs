using Antro.Content.Moscow2026;
using Antro.Domain;

namespace Antro.Application.Tests;

public sealed class DemoProfilesTests
{
    private static readonly EvaluationContext Context = new(
        Today: new DateOnly(2026, 5, 23),
        Region: Moscow2026BenefitCatalog.Region,
        CatalogVersion: Moscow2026BenefitCatalog.CatalogVersion,
        Assumptions: new AssumedMvpEligibilityContext(true, true, true));

    [Fact]
    public void Maria_newborn_sets_child_date_14_days_before_today()
    {
        var today = new DateOnly(2026, 5, 23);

        var profile = DemoProfiles.MariaNewborn(today);

        Assert.Equal(new DateOnly(2026, 5, 9), profile.ChildDate);
        Assert.Equal(DateInputKind.BirthDate, profile.DateInputKind);
    }

    [Fact]
    public void Maria_newborn_sets_expected_categorical_values()
    {
        var profile = DemoProfiles.MariaNewborn(Context.Today);

        Assert.Equal(ChildOrder.First, profile.ChildOrder);
        Assert.Equal(MatCapitalHistory.NeverReceived, profile.MatCapitalHistory);
        Assert.Equal(FamilyStatus.Married, profile.FamilyStatus);
        Assert.Equal(EmploymentStatus.Both, profile.EmploymentStatus);
        Assert.Equal(ParentAgeBand.BothUnderThirtySix, profile.ParentAgeBand);
        Assert.Equal(IncomeBand.UpToOneLivingMinimum, profile.IncomeBand);
        Assert.Equal(PropertyStatus.OwnsHome, profile.PropertyStatus);
        Assert.Equal(MortgageIntent.PlansToUseMortgage, profile.MortgageIntent);
    }

    [Fact]
    public void Demo_profile_uses_normal_dashboard_builder_path()
    {
        var builder = new DashboardBuilder();
        var profile = DemoProfiles.MariaNewborn(Context.Today);

        var dashboard = builder.Build(Moscow2026BenefitCatalog.All, profile, Context);

        Assert.NotEmpty(dashboard.AllBenefitSections);
        var allCards = dashboard.AllBenefitSections.SelectMany(s => s.Cards).ToArray();
        Assert.Contains(allCards, item => item.BenefitId == Moscow2026BenefitCatalog.MoscowYoungFamilyPayment.Id);
        Assert.Contains(allCards, item => item.BenefitId == Moscow2026BenefitCatalog.FederalBirthGrant.Id);
    }
}
