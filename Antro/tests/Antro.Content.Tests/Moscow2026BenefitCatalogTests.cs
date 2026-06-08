using Antro.Content.Moscow2026;
using Antro.Domain;

namespace Antro.Content.Tests;

public sealed class Moscow2026BenefitCatalogTests
{
    [Fact]
    public void Thresholds_can_be_instantiated_with_explicit_rub_money_values()
    {
        var thresholds = new Moscow2026Thresholds(
            livingMinimumPerCapita: new MoneyAmount(25_342m, Currency.Rub, AmountPeriod.Monthly),
            livingMinimumWorkingAge: new MoneyAmount(28_940m, Currency.Rub, AmountPeriod.Monthly),
            livingMinimumChild: new MoneyAmount(21_903m, Currency.Rub, AmountPeriod.Monthly),
            unifiedBenefitIncomeThreshold: new MoneyAmount(25_342m, Currency.Rub, AmountPeriod.Monthly),
            annualFamilyPaymentThreshold: new MoneyAmount(43_410m, Currency.Rub, AmountPeriod.Monthly),
            matCapitalMonthlyPaymentThreshold: new MoneyAmount(50_684m, Currency.Rub, AmountPeriod.Monthly));

        Assert.Equal(Currency.Rub, thresholds.LivingMinimumPerCapita.Currency);
        Assert.Equal(Currency.Rub, thresholds.LivingMinimumWorkingAge.Currency);
        Assert.Equal(Currency.Rub, thresholds.LivingMinimumChild.Currency);
        Assert.Equal(Currency.Rub, thresholds.UnifiedBenefitIncomeThreshold.Currency);
        Assert.Equal(Currency.Rub, thresholds.AnnualFamilyPaymentThreshold.Currency);
        Assert.Equal(Currency.Rub, thresholds.MatCapitalMonthlyPaymentThreshold.Currency);
    }

    [Fact]
    public void Catalog_contains_exactly_32_benefits()
    {
        Assert.Equal(32, Moscow2026BenefitCatalog.All.Count);
    }

    [Fact]
    public void Catalog_all_contains_no_null_entries_and_readable_ids_and_slugs()
    {
        var allBenefits = Moscow2026BenefitCatalog.All;

        Assert.Equal(32, allBenefits.Count);
        Assert.DoesNotContain(allBenefits, benefit => benefit is null);
        Assert.All(allBenefits, benefit =>
        {
            Assert.False(string.IsNullOrWhiteSpace(benefit.Id.Value));
            Assert.False(string.IsNullOrWhiteSpace(benefit.Slug));
        });
    }

    [Fact]
    public void Catalog_static_properties_and_all_collection_match()
    {
        var staticBenefits = typeof(Moscow2026BenefitCatalog)
            .GetProperties()
            .Where(property => property.PropertyType == typeof(Benefit))
            .Select(property => (Benefit)property.GetValue(null)!)
            .ToArray();

        Assert.Equal(32, staticBenefits.Length);
        Assert.Equal(
            staticBenefits.Select(benefit => benefit.Id.Value).OrderBy(value => value),
            Moscow2026BenefitCatalog.All.Select(benefit => benefit.Id.Value).OrderBy(value => value));
    }

    [Fact]
    public void All_benefit_ids_are_unique_and_match_the_known_catalog_ids()
    {
        var ids = Moscow2026BenefitCatalog.All.Select(benefit => benefit.Id.Value).ToArray();
        var expectedIds = Moscow2026BenefitIds.All.Select(id => id.Value).ToArray();

        Assert.Equal(ids.Length, ids.Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(expectedIds.OrderBy(value => value), ids.OrderBy(value => value));
    }

    [Fact]
    public void All_slugs_are_unique_and_non_empty()
    {
        var slugs = Moscow2026BenefitCatalog.All.Select(benefit => benefit.Slug).ToArray();

        Assert.All(slugs, slug => Assert.False(string.IsNullOrWhiteSpace(slug)));
        Assert.Equal(slugs.Length, slugs.Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void Every_benefit_has_required_catalog_content()
    {
        Assert.All(Moscow2026BenefitCatalog.All, benefit =>
        {
            Assert.Equal(Moscow2026BenefitCatalog.Region, benefit.Region);
            Assert.Equal(Moscow2026BenefitCatalog.CatalogVersion, benefit.CatalogVersion);
            Assert.NotNull(benefit.Copy);
            Assert.False(string.IsNullOrWhiteSpace(benefit.Copy.Title));
            Assert.False(string.IsNullOrWhiteSpace(benefit.Copy.ShortDescription));
            Assert.False(string.IsNullOrWhiteSpace(benefit.Copy.DetailedDescription));
            Assert.NotNull(benefit.AmountRule);
            Assert.NotEmpty(benefit.StageAvailability);
            Assert.NotEmpty(benefit.Documents);
            Assert.NotEmpty(benefit.ActionSteps);
            Assert.NotEmpty(benefit.LegalSources);
        });
    }

    [Fact]
    public void Every_deadline_event_has_a_deadline_rule()
    {
        var deadlineEvents = Moscow2026BenefitCatalog.All
            .Where(benefit => benefit.Type == BenefitType.DeadlineEvent)
            .ToArray();

        Assert.NotEmpty(deadlineEvents);

        Assert.All(deadlineEvents, benefit =>
        {
            Assert.NotNull(benefit.DeadlineRule);

            if (benefit.DeadlineRule is NoDeadline noDeadline)
            {
                Assert.False(string.IsNullOrWhiteSpace(noDeadline.Notes));
            }
        });
    }

    [Fact]
    public void Expired_or_future_visibility_is_represented_through_stage_availability()
    {
        Assert.Contains(
            Moscow2026BenefitCatalog.MoscowNewbornGiftSet.StageAvailability,
            stage => stage.LifeStage == LifeStage.UpToSixMonths && stage.Status == StageAvailabilityStatus.Expired);

        Assert.Contains(
            Moscow2026BenefitCatalog.FederalBirthGrant.StageAvailability,
            stage => stage.LifeStage == LifeStage.UpToEighteenMonths && stage.Status == StageAvailabilityStatus.Expired);

        Assert.Contains(
            Moscow2026BenefitCatalog.ChildMilkKitchen.StageAvailability,
            stage => stage.LifeStage == LifeStage.OlderThanThreeYears && stage.Status == StageAvailabilityStatus.Expired);

        Assert.Contains(
            Moscow2026BenefitCatalog.SchoolMeals.StageAvailability,
            stage => stage.LifeStage == LifeStage.UpToThreeYears && stage.Status == StageAvailabilityStatus.Future);
    }

    [Fact]
    public void Compatibility_catalog_wrapper_points_to_the_typed_catalog()
    {
        Assert.Equal(Moscow2026BenefitCatalog.Region, Moscow2026Catalog.Region);
        Assert.Equal(Moscow2026BenefitCatalog.CatalogVersion, Moscow2026Catalog.CatalogVersion);
        Assert.Equal(Moscow2026BenefitCatalog.Thresholds, Moscow2026Catalog.Thresholds);
        Assert.Equal(32, Moscow2026Catalog.GetBenefits().Count);
    }

    [Fact]
    public void Matcapital_monthly_payment_uses_child_living_minimum_amount()
    {
        var amountRule = Assert.IsType<FixedAmount>(Moscow2026BenefitCatalog.MatCapitalMonthlyPayment.AmountRule);

        Assert.Equal(Moscow2026BenefitCatalog.Thresholds.LivingMinimumChild, amountRule.Amount);
    }

    [Fact]
    public void Rule_driven_benefits_have_concrete_eligibility_rules_attached()
    {
        Assert.NotEmpty(Moscow2026BenefitCatalog.PregnancyMonthlyBenefit.EligibilityRules);
        Assert.NotEmpty(Moscow2026BenefitCatalog.MoscowYoungFamilyPayment.EligibilityRules);
        Assert.NotEmpty(Moscow2026BenefitCatalog.UnifiedChildBenefit.EligibilityRules);
        Assert.NotEmpty(Moscow2026BenefitCatalog.MaternityCapital.EligibilityRules);
        Assert.NotEmpty(Moscow2026BenefitCatalog.FamilyMortgage.EligibilityRules);
        Assert.NotEmpty(Moscow2026BenefitCatalog.FederalYoungFamilyHousingProgram.EligibilityRules);
        Assert.NotEmpty(Moscow2026BenefitCatalog.ChildTaxDeduction.EligibilityRules);
    }

    [Fact]
    public void All_income_dependent_benefits_use_income_band_rules()
    {
        var incomeDependentBenefits = Moscow2026BenefitCatalog.All
            .Where(benefit => benefit.Type == BenefitType.IncomeDependentPayment)
            .ToArray();

        Assert.NotEmpty(incomeDependentBenefits);
        Assert.All(
            incomeDependentBenefits,
            benefit => Assert.Contains(benefit.EligibilityRules, rule => rule is IncomeBandRule));
    }
}
