using Antro.Application;
using Antro.Content.Moscow2026;
using Antro.Domain;

namespace Antro.Application.Tests;

public sealed class ViewModelBuildersTests
{
    private static readonly EvaluationContext Context = new(
        Today: new DateOnly(2026, 5, 23),
        Region: Moscow2026BenefitCatalog.Region,
        CatalogVersion: Moscow2026BenefitCatalog.CatalogVersion,
        Assumptions: new AssumedMvpEligibilityContext(true, true, true));

    [Fact]
    public void Newborn_stage_showcase_returns_sorted_cards_for_key_newborn_measures()
    {
        var builder = new StageShowcaseBuilder();

        var showcase = builder.Build(
            LifeStage.NewbornZeroToTwoMonths,
            Moscow2026BenefitCatalog.All,
            Context,
            profile: new UserProfile());

        Assert.Equal(LifeStage.NewbornZeroToTwoMonths, showcase.Stage);
        Assert.False(string.IsNullOrWhiteSpace(showcase.Title));
        Assert.False(string.IsNullOrWhiteSpace(showcase.Subtitle));
        Assert.NotEmpty(showcase.Sections);

        var allCards = showcase.Sections.SelectMany(s => s.Cards).ToArray();
        var cardIds = allCards.Select(card => card.BenefitId).ToArray();
        Assert.Contains(Moscow2026BenefitCatalog.MoscowNewbornGiftSet.Id, cardIds);
        Assert.Contains(Moscow2026BenefitCatalog.FederalBirthGrant.Id, cardIds);
        Assert.Contains(Moscow2026BenefitCatalog.MoscowBirthPayment.Id, cardIds);
        Assert.Contains(Moscow2026BenefitCatalog.MaternityCapital.Id, cardIds);
        Assert.Contains(Moscow2026BenefitCatalog.ChildCareLeaveRight.Id, cardIds);

        Assert.True(IndexOf(cardIds, Moscow2026BenefitCatalog.MoscowNewbornGiftSet.Id) < IndexOf(cardIds, Moscow2026BenefitCatalog.MaternityCapital.Id));
    }

    [Fact]
    public void Stage_showcase_works_with_incomplete_profile_without_claiming_false_precise_eligibility()
    {
        var builder = new StageShowcaseBuilder();

        var showcase = builder.Build(
            LifeStage.NewbornZeroToTwoMonths,
            Moscow2026BenefitCatalog.All,
            Context,
            profile: new UserProfile());

        var unifiedBenefit = showcase.Sections.SelectMany(s => s.Cards).Single(card => card.BenefitId == Moscow2026BenefitCatalog.UnifiedChildBenefit.Id);

        Assert.Equal("needs-more-info", unifiedBenefit.Availability.SemanticKey);
        Assert.Contains("нужны ответы анкеты", showcase.SummaryText, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Dashboard_builder_returns_summary_urgent_deadlines_current_rights_and_all_benefits()
    {
        var builder = new DashboardBuilder();
        var mariaProfile = DemoProfiles.MariaNewborn(Context.Today);

        var dashboard = builder.Build(Moscow2026BenefitCatalog.All, mariaProfile, Context);

        Assert.False(string.IsNullOrWhiteSpace(dashboard.Title));
        Assert.False(string.IsNullOrWhiteSpace(dashboard.Stage.Label));
        Assert.True(dashboard.Summary.TotalBenefitsCount >= 31);
        Assert.NotEmpty(dashboard.UrgentDeadlines);
        Assert.NotEmpty(dashboard.CurrentRights);
        Assert.True(dashboard.AllBenefitSections.Sum(s => s.Cards.Count) >= 31);
        Assert.Contains(dashboard.UrgentDeadlines, item => item.BenefitId == Moscow2026BenefitCatalog.MoscowNewbornGiftSet.Id);
        Assert.Contains(dashboard.CurrentRights, item => item.BenefitId == Moscow2026BenefitCatalog.ChildCareLeaveRight.Id);
    }

    [Fact]
    public void Benefit_detail_builder_returns_full_detail_for_young_family_payment()
    {
        var builder = new BenefitDetailBuilder();
        var detail = builder.Build(
            Moscow2026BenefitCatalog.MoscowYoungFamilyPayment,
            DemoProfiles.MariaNewborn(Context.Today),
            Context);

        Assert.Equal(Moscow2026BenefitCatalog.MoscowYoungFamilyPayment.Id, detail.BenefitId);
        Assert.False(string.IsNullOrWhiteSpace(detail.Title));
        Assert.False(string.IsNullOrWhiteSpace(detail.Subtitle));
        Assert.NotNull(detail.Amount);
        Assert.False(string.IsNullOrWhiteSpace(detail.Amount!.Text));
        Assert.False(string.IsNullOrWhiteSpace(detail.Deadline.Text));
        Assert.False(string.IsNullOrWhiteSpace(detail.WhatIsIt));
        Assert.NotEmpty(detail.Conditions);
        Assert.NotEmpty(detail.Documents);
        Assert.NotEmpty(detail.ActionSteps);
        Assert.NotEmpty(detail.LegalSources);
    }

    private static int IndexOf(IReadOnlyList<BenefitId> ids, BenefitId expectedId)
    {
        for (var index = 0; index < ids.Count; index++)
        {
            if (ids[index] == expectedId)
            {
                return index;
            }
        }

        return -1;
    }
}
