using Antro.Application;
using Antro.Content.Moscow2026;
using Antro.Domain;

namespace Antro.Content.Tests;

public sealed class Moscow2026CatalogEvaluationTests
{
    private static readonly IBenefitEvaluator Evaluator = new BenefitEvaluator();

    [Fact]
    public void Maria_newborn_profile_keeps_newborn_deadline_benefits_available()
    {
        var profile = new UserProfile
        {
            ChildDate = new DateOnly(2026, 5, 9),
            DateInputKind = DateInputKind.BirthDate,
            ChildOrder = ChildOrder.First,
            MatCapitalHistory = MatCapitalHistory.NeverReceived,
            FamilyStatus = FamilyStatus.Married,
            EmploymentStatus = EmploymentStatus.Both,
            ParentAgeBand = ParentAgeBand.BothUnderThirtySix,
            IncomeBand = IncomeBand.UpToOneLivingMinimum,
            PropertyStatus = PropertyStatus.OwnsHome,
            MortgageIntent = MortgageIntent.None
        };
        var results = EvaluateCatalog(
            profile);

        var prioritySorted = EvaluateBenefits(
            [
                Moscow2026BenefitCatalog.MaternityCapital,
                Moscow2026BenefitCatalog.MoscowYoungFamilyPayment,
                Moscow2026BenefitCatalog.FederalBirthGrant,
                Moscow2026BenefitCatalog.MoscowBirthPayment,
                Moscow2026BenefitCatalog.MoscowNewbornGiftSet
            ],
            profile);

        var orderedBenefitIds = prioritySorted.Select(result => result.BenefitId).ToArray();
        var giftSetIndex = Array.IndexOf(orderedBenefitIds, Moscow2026BenefitCatalog.MoscowNewbornGiftSet.Id);
        var matcapitalIndex = Array.IndexOf(orderedBenefitIds, Moscow2026BenefitCatalog.MaternityCapital.Id);
        var federalBirthGrantIndex = Array.IndexOf(orderedBenefitIds, Moscow2026BenefitCatalog.FederalBirthGrant.Id);
        var moscowBirthPaymentIndex = Array.IndexOf(orderedBenefitIds, Moscow2026BenefitCatalog.MoscowBirthPayment.Id);

        Assert.True(giftSetIndex >= 0);
        Assert.True(matcapitalIndex >= 0);
        Assert.True(federalBirthGrantIndex >= 0);
        Assert.True(moscowBirthPaymentIndex >= 0);
        Assert.True(giftSetIndex < matcapitalIndex);
        Assert.True(federalBirthGrantIndex < matcapitalIndex);
        Assert.True(moscowBirthPaymentIndex < matcapitalIndex);

        var giftSet = FindResult(results, Moscow2026BenefitCatalog.MoscowNewbornGiftSet);
        var federalBirthGrant = FindResult(results, Moscow2026BenefitCatalog.FederalBirthGrant);
        var youngFamilyPayment = FindResult(results, Moscow2026BenefitCatalog.MoscowYoungFamilyPayment);

        Assert.Equal(AvailabilityStatus.Available, giftSet.AvailabilityStatus);
        Assert.Contains(giftSet.DeadlineStatus, new[] { DeadlineStatus.Active, DeadlineStatus.Soon, DeadlineStatus.Urgent });

        Assert.Contains(
            federalBirthGrant.AvailabilityStatus,
            new[] { AvailabilityStatus.Available, AvailabilityStatus.PotentiallyAvailable });
        Assert.Contains(
            federalBirthGrant.DeadlineStatus,
            new[] { DeadlineStatus.Active, DeadlineStatus.Soon, DeadlineStatus.Urgent });

        Assert.Contains(
            youngFamilyPayment.AvailabilityStatus,
            new[] { AvailabilityStatus.Available, AvailabilityStatus.PotentiallyAvailable });
    }

    [Fact]
    public void High_income_profile_blocks_income_dependent_benefits()
    {
        var results = EvaluateCatalog(
            new UserProfile
            {
                ChildDate = new DateOnly(2026, 5, 9),
                DateInputKind = DateInputKind.BirthDate,
                ChildOrder = ChildOrder.First,
                EmploymentStatus = EmploymentStatus.Both,
                ParentAgeBand = ParentAgeBand.BothUnderThirtySix,
                IncomeBand = IncomeBand.AboveTwoLivingMinimums,
                PropertyStatus = PropertyStatus.OwnsHome
            });

        Assert.Equal(AvailabilityStatus.Unavailable, FindResult(results, Moscow2026BenefitCatalog.UnifiedChildBenefit).AvailabilityStatus);
        Assert.Equal(AvailabilityStatus.Unavailable, FindResult(results, Moscow2026BenefitCatalog.MatCapitalMonthlyPayment).AvailabilityStatus);
    }

    [Fact]
    public void Unknown_income_profile_requests_more_information_for_income_dependent_benefits()
    {
        var profile = new UserProfile
        {
            ChildDate = new DateOnly(2026, 5, 9),
            DateInputKind = DateInputKind.BirthDate,
            ChildOrder = ChildOrder.First,
            EmploymentStatus = EmploymentStatus.Both,
            ParentAgeBand = ParentAgeBand.BothUnderThirtySix,
            PropertyStatus = PropertyStatus.OwnsHome
        };
        var results = EvaluateCatalog(
            profile);
        var prioritySorted = EvaluateBenefits(
            [
                Moscow2026BenefitCatalog.UnifiedChildBenefit,
                Moscow2026BenefitCatalog.FederalBirthGrant,
                Moscow2026BenefitCatalog.MoscowNewbornGiftSet
            ],
            profile);
        var orderedBenefitIds = prioritySorted.Select(result => result.BenefitId).ToArray();

        Assert.True(Array.IndexOf(orderedBenefitIds, Moscow2026BenefitCatalog.UnifiedChildBenefit.Id) >= 0);
        Assert.True(Array.IndexOf(orderedBenefitIds, Moscow2026BenefitCatalog.MoscowNewbornGiftSet.Id) < Array.IndexOf(orderedBenefitIds, Moscow2026BenefitCatalog.UnifiedChildBenefit.Id));

        var unifiedBenefit = FindResult(results, Moscow2026BenefitCatalog.UnifiedChildBenefit);

        Assert.Equal(AvailabilityStatus.NeedsMoreInfo, unifiedBenefit.AvailabilityStatus);
        Assert.Contains(MissingProfileField.IncomeBand, unifiedBenefit.MissingFields);
    }

    [Fact]
    public void Second_child_matcapital_scenario_reacts_to_child_order_and_history()
    {
        var secondChildFirstCertificate = EvaluateCatalog(
            new UserProfile
            {
                ChildDate = new DateOnly(2026, 5, 9),
                DateInputKind = DateInputKind.BirthDate,
                ChildOrder = ChildOrder.Second,
                MatCapitalHistory = MatCapitalHistory.NeverReceived,
                IncomeBand = IncomeBand.UpToOneLivingMinimum,
                EmploymentStatus = EmploymentStatus.Both
            });

        var secondChildAdditionalAmount = EvaluateCatalog(
            new UserProfile
            {
                ChildDate = new DateOnly(2026, 5, 9),
                DateInputKind = DateInputKind.BirthDate,
                ChildOrder = ChildOrder.Second,
                MatCapitalHistory = MatCapitalHistory.PartiallyUsed,
                IncomeBand = IncomeBand.UpToOneLivingMinimum,
                EmploymentStatus = EmploymentStatus.Both
            });

        var unavailableWhenFullyUsed = EvaluateCatalog(
            new UserProfile
            {
                ChildDate = new DateOnly(2026, 5, 9),
                DateInputKind = DateInputKind.BirthDate,
                ChildOrder = ChildOrder.Second,
                MatCapitalHistory = MatCapitalHistory.FullyUsed,
                IncomeBand = IncomeBand.UpToOneLivingMinimum,
                EmploymentStatus = EmploymentStatus.Both
            });

        var baseCapital = FindResult(secondChildFirstCertificate, Moscow2026BenefitCatalog.MaternityCapital);
        var extraCapital = FindResult(secondChildAdditionalAmount, Moscow2026BenefitCatalog.MaternityCapital);
        var rejectedCapital = FindResult(unavailableWhenFullyUsed, Moscow2026BenefitCatalog.MaternityCapital);

        Assert.Equal(AvailabilityStatus.Automatic, baseCapital.AvailabilityStatus);
        Assert.Equal(new MoneyAmount(963_243.17m, Currency.Rub, AmountPeriod.OneTime), baseCapital.MoneyEstimate!.ExactAmount);

        Assert.Equal(AvailabilityStatus.Automatic, extraCapital.AvailabilityStatus);
        Assert.Equal(new MoneyAmount(234_321.27m, Currency.Rub, AmountPeriod.OneTime), extraCapital.MoneyEstimate!.ExactAmount);

        Assert.Equal(AvailabilityStatus.Unavailable, rejectedCapital.AvailabilityStatus);
    }

    [Fact]
    public void Unknown_property_status_does_not_create_false_denial()
    {
        var results = EvaluateCatalog(
            new UserProfile
            {
                ChildDate = new DateOnly(2026, 5, 9),
                DateInputKind = DateInputKind.BirthDate,
                ChildOrder = ChildOrder.First,
                EmploymentStatus = EmploymentStatus.Both,
                IncomeBand = IncomeBand.UpToOneLivingMinimum
            });

        var unifiedBenefit = FindResult(results, Moscow2026BenefitCatalog.UnifiedChildBenefit);

        Assert.Equal(AvailabilityStatus.NeedsMoreInfo, unifiedBenefit.AvailabilityStatus);
        Assert.NotEqual(AvailabilityStatus.Unavailable, unifiedBenefit.AvailabilityStatus);
        Assert.Contains(MissingProfileField.PropertyStatus, unifiedBenefit.MissingFields);
    }

    [Fact]
    public void Expired_benefit_remains_visible_as_expired()
    {
        var profile = new UserProfile
        {
            ChildDate = new DateOnly(2026, 2, 20),
            DateInputKind = DateInputKind.BirthDate,
            ChildOrder = ChildOrder.First,
            MatCapitalHistory = MatCapitalHistory.NeverReceived,
            EmploymentStatus = EmploymentStatus.Both,
            IncomeBand = IncomeBand.UpToOneLivingMinimum,
            PropertyStatus = PropertyStatus.OwnsHome
        };
        var results = EvaluateCatalog(profile);
        var prioritySorted = EvaluateBenefits(
            [
                Moscow2026BenefitCatalog.MoscowNewbornGiftSet,
                Moscow2026BenefitCatalog.MaternityCapital
            ],
            profile);

        var giftSet = FindResult(results, Moscow2026BenefitCatalog.MoscowNewbornGiftSet);

        Assert.Equal(AvailabilityStatus.Expired, giftSet.AvailabilityStatus);
        Assert.Equal(DeadlineStatus.Expired, giftSet.DeadlineStatus);
        Assert.Contains(prioritySorted, result => result.BenefitId == Moscow2026BenefitCatalog.MoscowNewbornGiftSet.Id);
        Assert.Equal(Moscow2026BenefitCatalog.MaternityCapital.Id, prioritySorted[0].BenefitId);
    }

    private static IReadOnlyList<EvaluatedBenefit> EvaluateCatalog(
        UserProfile profile,
        AssumedMvpEligibilityContext? assumptions = null,
        DateOnly? today = null)
    {
        var context = new EvaluationContext(
            today ?? new DateOnly(2026, 5, 23),
            Moscow2026BenefitCatalog.Region,
            Moscow2026BenefitCatalog.CatalogVersion,
            assumptions ?? new AssumedMvpEligibilityContext(true, true, true));

        return Evaluator.Evaluate(Moscow2026BenefitCatalog.All, profile, context);
    }

    private static IReadOnlyList<EvaluatedBenefit> EvaluateBenefits(
        IReadOnlyList<Benefit> benefits,
        UserProfile profile,
        AssumedMvpEligibilityContext? assumptions = null,
        DateOnly? today = null)
    {
        var context = new EvaluationContext(
            today ?? new DateOnly(2026, 5, 23),
            Moscow2026BenefitCatalog.Region,
            Moscow2026BenefitCatalog.CatalogVersion,
            assumptions ?? new AssumedMvpEligibilityContext(true, true, true));

        return Evaluator.Evaluate(benefits, profile, context);
    }

    private static EvaluatedBenefit FindResult(IReadOnlyList<EvaluatedBenefit> results, Benefit benefit) =>
        results.Single(result => result.BenefitId == benefit.Id);
}
