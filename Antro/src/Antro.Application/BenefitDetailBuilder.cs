using Antro.Domain;

namespace Antro.Application;

public sealed class BenefitDetailBuilder : IBenefitDetailBuilder
{
    private readonly IBenefitEvaluator evaluator;

    public BenefitDetailBuilder(IBenefitEvaluator? evaluator = null)
    {
        this.evaluator = evaluator ?? new BenefitEvaluator();
    }

    public BenefitDetailViewModel Build(
        Benefit benefit,
        UserProfile profile,
        EvaluationContext context)
    {
        ArgumentNullException.ThrowIfNull(benefit);
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(context);

        var evaluatedBenefit = evaluator.Evaluate([benefit], profile, context).Single();
        var conditions = benefit.EligibilityRules
            .Select(rule =>
            {
                var result = rule.Evaluate(profile, context);
                return new ConditionViewModel(
                    Text: ApplicationDisplayText.GetConditionLabel(rule),
                    Status: ApplicationDisplayText.GetRuleOutcomePill(result.Outcome),
                    Explanation: result.Explanation);
            })
            .ToArray();

        return new BenefitDetailViewModel(
            benefit.Id,
            benefit.Slug,
            benefit.Copy.Title,
            benefit.Copy.OfficialTitle ?? benefit.Copy.ShortDescription,
            ApplicationDisplayText.GetAvailabilityPill(evaluatedBenefit.AvailabilityStatus),
            BenefitViewModelMapper.ToAmountDisplay(evaluatedBenefit.MoneyEstimate),
            BenefitViewModelMapper.ToDeadlineBadge(evaluatedBenefit.Deadline),
            benefit.Copy.DetailedDescription,
            conditions,
            benefit.Documents
                .Select(document => new DocumentRequirementViewModel(document.Title, document.Description, document.Notes))
                .ToArray(),
            benefit.ActionSteps
                .OrderBy(step => step.Order)
                .Select(step => new ActionStepViewModel(step.Order, step.Title, step.Description, step.Notes))
                .ToArray(),
            benefit.LegalSources
                .Select(source => new LegalSourceViewModel(
                    source.Title,
                    source.ActNumberOrIdentifier,
                    source.ArticleOrClause,
                    ApplicationDisplayText.GetLegalSourceLevelLabel(source.Level),
                    ApplicationDisplayText.GetVerificationLabel(source.VerificationStatus),
                    source.LastCheckedAt,
                    source.Notes))
                .ToArray());
    }
}
