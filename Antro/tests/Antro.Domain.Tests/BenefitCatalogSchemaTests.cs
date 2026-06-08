using Antro.Domain;

namespace Antro.Domain.Tests;

public sealed class BenefitCatalogSchemaTests
{
    [Fact]
    public void Benefit_can_be_constructed_with_domain_content_types()
    {
        var benefit = new Benefit(
            id: new BenefitId("benefit.sample"),
            slug: "sample-benefit",
            region: new RegionCode("RU-MOW"),
            type: BenefitType.IncomeDependentPayment,
            tier: BenefitTier.Tier1Core,
            category: BenefitCategory.FamilyIncomeSupport,
            copy: new BenefitCopy(
                title: "Sample family payment",
                officialTitle: "Official sample family payment",
                shortDescription: "Short showcase text.",
                detailedDescription: "Detailed what-is-it text for the benefit card.",
                notes: "Optional note."),
            amountRule: new FixedAmount(new MoneyAmount(15_000m, Currency.Rub, AmountPeriod.Monthly)),
            deadlineRule: new FilingDeadlineFromBirth(6, "Apply within six months of birth."),
            stageAvailability:
            [
                new LifeStageAvailability(
                    lifeStage: LifeStage.UpToSixMonths,
                    isVisible: true,
                    status: StageAvailabilityStatus.Active,
                    explanation: "Normally active during early infancy.")
            ],
            eligibilityRules:
            [
                new StubEligibilityRule()
            ],
            documents:
            [
                new DocumentRequirement("Passport", "Applicant passport.", null)
            ],
            actionSteps:
            [
                new ActionStep(1, "Submit application", "Apply through the official channel.", null)
            ],
            legalSources:
            [
                new LegalSource(
                    title: "Sample Moscow law",
                    actNumberOrIdentifier: "123-PP",
                    articleOrClause: "Art. 2",
                    level: LegalSourceLevel.MoscowLaw,
                    lastCheckedAt: new DateOnly(2026, 5, 23),
                    verificationStatus: VerificationStatus.Checked,
                    notes: "Sample citation.")
            ]);

        Assert.Equal(new BenefitId("benefit.sample"), benefit.Id);
        Assert.Equal("sample-benefit", benefit.Slug);
        Assert.Equal(BenefitType.IncomeDependentPayment, benefit.Type);
        Assert.Equal("Sample family payment", benefit.Copy.Title);
        Assert.IsType<FixedAmount>(benefit.AmountRule);
        Assert.IsType<FilingDeadlineFromBirth>(benefit.DeadlineRule);
        Assert.Single(benefit.StageAvailability);
        Assert.Single(benefit.EligibilityRules);
        Assert.Single(benefit.Documents);
        Assert.Single(benefit.ActionSteps);
        Assert.Single(benefit.LegalSources);
    }

    [Fact]
    public void LegalSource_supports_draft_and_checked_verification_statuses()
    {
        var draftSource = new LegalSource(
            title: "Draft portal note",
            actNumberOrIdentifier: "portal-draft",
            articleOrClause: null,
            level: LegalSourceLevel.OfficialPortal,
            lastCheckedAt: null,
            verificationStatus: VerificationStatus.Draft,
            notes: "Awaiting legal confirmation.");

        var checkedSource = new LegalSource(
            title: "Checked federal law",
            actNumberOrIdentifier: "81-FZ",
            articleOrClause: "Art. 12",
            level: LegalSourceLevel.FederalLaw,
            lastCheckedAt: new DateOnly(2026, 5, 23),
            verificationStatus: VerificationStatus.Checked,
            notes: null);

        Assert.Equal(VerificationStatus.Draft, draftSource.VerificationStatus);
        Assert.Equal(VerificationStatus.Checked, checkedSource.VerificationStatus);
    }

    [Fact]
    public void Benefit_schema_does_not_expose_ui_or_css_only_fields()
    {
        var publicPropertyNames = typeof(Benefit)
            .GetProperties()
            .Select(property => property.Name)
            .ToArray();

        Assert.DoesNotContain(publicPropertyNames, name => name.Contains("Css", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(publicPropertyNames, name => name.Contains("Html", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(publicPropertyNames, name => name.Contains("Color", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(publicPropertyNames, name => name.Contains("Icon", StringComparison.OrdinalIgnoreCase));
    }

    private sealed class StubEligibilityRule : IEligibilityRule
    {
        public string Code => "stub-rule";

        public string Description => "Test-only stub rule.";

        public EligibilityCheckResult Evaluate(UserProfile profile, EvaluationContext context) =>
            new(Code, RuleOutcome.Pass);
    }
}
