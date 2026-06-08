using Antro.Domain;

namespace Antro.Application;

public sealed class StageShowcaseBuilder : IStageShowcaseBuilder
{
    private readonly IBenefitEvaluator evaluator;

    public StageShowcaseBuilder(IBenefitEvaluator? evaluator = null)
    {
        this.evaluator = evaluator ?? new BenefitEvaluator();
    }

    public StageShowcaseViewModel Build(
        LifeStage selectedStage,
        IReadOnlyList<Benefit> catalog,
        EvaluationContext context,
        UserProfile? profile = null)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(context);

        var effectiveProfile = StageProfileFactory.ApplyStageAnchor(selectedStage, profile ?? new UserProfile(), context.Today);
        var stageBenefits = catalog
            .Where(benefit => ResolveStageAvailability(benefit, selectedStage)?.IsVisible == true)
            .ToArray();
        var evaluations = evaluator.Evaluate(stageBenefits, effectiveProfile, context);
        var cards = stageBenefits
            .Join(
                evaluations,
                benefit => benefit.Id,
                evaluation => evaluation.BenefitId,
                BenefitViewModelMapper.ToCard)
            .OrderByDescending(card => card.PriorityScore)
            .ToArray();

        var summaryText = cards.Any(card => card.Availability.SemanticKey == "needs-more-info")
            ? "Показываем актуальные меры этого этапа. Для части карточек нужны ответы анкеты."
            : "Показываем актуальные меры этого этапа в порядке риска упустить и практической пользы.";

        var sections = BenefitSectionGrouper.Group(cards);
        var headerInfo = BuildHeaderInfo(sections);

        return new StageShowcaseViewModel(
            selectedStage,
            ApplicationDisplayText.GetStageTitle(selectedStage),
            ApplicationDisplayText.GetStageSubtitle(selectedStage),
            SummaryText: summaryText,
            Sections: sections,
            HeaderInfo: headerInfo);
    }

    private static StageShowcaseHeaderInfoViewModel BuildHeaderInfo(IReadOnlyList<BenefitSectionViewModel> sections)
    {
        var urgentCount = sections.FirstOrDefault(s => s.SectionKey == "urgent")?.Cards.Count ?? 0;
        var rightsCount = sections.FirstOrDefault(s => s.SectionKey == "rights")?.Cards.Count ?? 0;

        // Суммы — хардкод для мокапа. MoneyEstimate имеет слишком разные виды
        // (Range, UpTo, Choice) для единого числового агрегата.
        var amountsByCategory = new AmountCategoryViewModel[]
        {
            new("Разовые выплаты",         "≈ 183 000 ₽"),
            new("Ежемесячно (до 1,5 лет)", "до 105 000 ₽"),
            new("Материнский капитал",      "728 900 ₽"),
        };

        return new StageShowcaseHeaderInfoViewModel(
            UrgentCount:       urgentCount,
            RightsCount:       rightsCount,
            RegionName:        "Москва",
            TotalAmountText:   "до ≈ 1 200 000 ₽",
            AmountsByCategory: amountsByCategory);
    }

    private static LifeStageAvailability? ResolveStageAvailability(Benefit benefit, LifeStage selectedStage)
    {
        var explicitAvailability = benefit.StageAvailability.FirstOrDefault(stage => stage.LifeStage == selectedStage);
        if (explicitAvailability is not null)
        {
            return explicitAvailability;
        }

        if (benefit.StageAvailability.Count == 0)
        {
            return null;
        }

        var orderedStages = benefit.StageAvailability
            .OrderBy(stage => (int)stage.LifeStage)
            .ToArray();
        var earliestStage = orderedStages[0];
        var latestStage = orderedStages[^1];

        if ((int)selectedStage < (int)earliestStage.LifeStage)
        {
            return new LifeStageAvailability(
                selectedStage,
                earliestStage.IsVisible,
                StageAvailabilityStatus.Future,
                earliestStage.Explanation);
        }

        if ((int)selectedStage > (int)latestStage.LifeStage)
        {
            return new LifeStageAvailability(
                selectedStage,
                latestStage.IsVisible,
                latestStage.Status == StageAvailabilityStatus.Informational ? StageAvailabilityStatus.Informational : StageAvailabilityStatus.Expired,
                latestStage.Explanation);
        }

        return null;
    }

    private static class StageProfileFactory
    {
        public static UserProfile ApplyStageAnchor(LifeStage stage, UserProfile profile, DateOnly today)
        {
            ArgumentNullException.ThrowIfNull(profile);

            if (profile.ChildDate is not null && profile.DateInputKind != DateInputKind.Unknown)
            {
                return profile;
            }

            var anchored = stage switch
            {
                LifeStage.ExpectingChild => new StageAnchor(today.AddMonths(4), DateInputKind.DueDate),
                LifeStage.NewbornZeroToTwoMonths => new StageAnchor(today.AddDays(-14), DateInputKind.BirthDate),
                LifeStage.UpToSixMonths => new StageAnchor(today.AddMonths(-4), DateInputKind.BirthDate),
                LifeStage.UpToEighteenMonths => new StageAnchor(today.AddMonths(-12), DateInputKind.BirthDate),
                LifeStage.UpToThreeYears => new StageAnchor(today.AddYears(-2), DateInputKind.BirthDate),
                LifeStage.OlderThanThreeYears => new StageAnchor(today.AddYears(-4), DateInputKind.BirthDate),
                _ => new StageAnchor(today.AddDays(-14), DateInputKind.BirthDate)
            };

            return profile with
            {
                ChildDate = profile.ChildDate ?? anchored.ChildDate,
                DateInputKind = profile.DateInputKind == DateInputKind.Unknown ? anchored.DateInputKind : profile.DateInputKind
            };
        }

        private sealed record StageAnchor(DateOnly ChildDate, DateInputKind DateInputKind);
    }
}
