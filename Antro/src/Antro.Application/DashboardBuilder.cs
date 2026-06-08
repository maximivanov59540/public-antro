using Antro.Domain;

namespace Antro.Application;

public sealed class DashboardBuilder : IDashboardBuilder
{
    private static readonly AvailabilityStatus[] SummaryEligibleStatuses =
    [
        AvailabilityStatus.Available,
        AvailabilityStatus.Automatic
    ];

    private static readonly DeadlineStatus[] DashboardDeadlineStatuses =
    [
        DeadlineStatus.Urgent,
        DeadlineStatus.Soon,
        DeadlineStatus.Active
    ];

    private readonly IBenefitEvaluator evaluator;

    public DashboardBuilder(IBenefitEvaluator? evaluator = null)
    {
        this.evaluator = evaluator ?? new BenefitEvaluator();
    }

    public DashboardViewModel Build(
        IReadOnlyList<Benefit> catalog,
        UserProfile profile,
        EvaluationContext context)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(context);

        var evaluations = evaluator.Evaluate(catalog, profile, context);
        var benefitById = catalog.ToDictionary(benefit => benefit.Id);
        var evaluatedBenefits = evaluations
            .Select(evaluation => new EvaluatedDashboardBenefit(benefitById[evaluation.BenefitId], evaluation))
            .ToArray();

        var cards = evaluatedBenefits
            .Select(item => BenefitViewModelMapper.ToCard(item.Benefit, item.Evaluation))
            .OrderByDescending(c => c.PriorityScore)
            .ToArray();
        var allBenefitSections = BenefitSectionGrouper.Group(cards);

        var urgentDeadlines = evaluatedBenefits
            .Where(item => DashboardDeadlineStatuses.Contains(item.Evaluation.DeadlineStatus))
            .Select(item => BenefitViewModelMapper.ToDeadlinePreview(item.Benefit, item.Evaluation))
            .Take(6)
            .ToArray();

        var currentRights = evaluatedBenefits
            .Where(item => item.Evaluation.BenefitType is BenefitType.OngoingRight or BenefitType.InformationalRight)
            .Where(item => item.Evaluation.AvailabilityStatus is AvailabilityStatus.Available
                or AvailabilityStatus.Automatic
                or AvailabilityStatus.Informational
                or AvailabilityStatus.PotentiallyAvailable)
            .Select(item => BenefitViewModelMapper.ToRightPreview(item.Benefit, item.Evaluation))
            .Take(6)
            .ToArray();

        var summary = new DashboardSummaryViewModel(
            $"Показываем {cards.Length} мер. В итогах суммируем только точные денежные суммы: диапазоны, лимиты и неденежные меры остаются отдельными.",
            BuildSummaryBuckets(evaluatedBenefits),
            TotalBenefitsCount: cards.Length,
            AvailableNowCount: evaluations.Count(result => result.AvailabilityStatus is AvailabilityStatus.Available or AvailabilityStatus.Automatic or AvailabilityStatus.PotentiallyAvailable),
            NeedsMoreInfoCount: evaluations.Count(result => result.AvailabilityStatus == AvailabilityStatus.NeedsMoreInfo),
            UrgentCount: evaluations.Count(result => DashboardDeadlineStatuses.Contains(result.DeadlineStatus)));

        return new DashboardViewModel(
            Title: "Ваш обзор мер поддержки",
            Stage: ResolveStage(profile, context.Today),
            Summary: summary,
            UrgentDeadlines: urgentDeadlines,
            CurrentRights: currentRights,
            AllBenefitSections: allBenefitSections);
    }

    private static DashboardStageViewModel ResolveStage(UserProfile profile, DateOnly today)
    {
        if (profile.ChildDate is null || profile.DateInputKind == DateInputKind.Unknown)
        {
            return new DashboardStageViewModel(
                "Текущий этап семьи",
                "Этап семьи уточняется",
                "Часть статусов и сроков зависит от точной даты рождения ребёнка или срока беременности.");
        }

        try
        {
            var stage = ChildLifeStageCalculator.Classify(profile.ChildDate.Value, profile.DateInputKind, today);
            return new DashboardStageViewModel(
                "Текущий этап семьи",
                ApplicationDisplayText.GetStageLabel(stage),
                ApplicationDisplayText.GetStageSubtitle(stage));
        }
        catch (ArgumentException)
        {
            return new DashboardStageViewModel(
                "Текущий этап семьи",
                "Этап семьи уточняется",
                "Часть статусов и сроков зависит от точной даты рождения ребёнка или срока беременности.");
        }
    }

    private static IReadOnlyList<DashboardSummaryBucketViewModel> BuildSummaryBuckets(
        IReadOnlyList<EvaluatedDashboardBenefit> evaluatedBenefits)
    {
        var buckets = new List<DashboardSummaryBucketViewModel>
        {
            BuildMoneyBucket(
                "Разовые выплаты",
                "one-time",
                evaluatedBenefits.Where(item => IsSummaryEligible(item.Evaluation)
                    && item.Evaluation.MoneyEstimate?.IsMonetary == true
                    && item.Evaluation.MoneyEstimate.Period == AmountPeriod.OneTime)),
            BuildMoneyBucket(
                "Ежемесячно",
                "monthly",
                evaluatedBenefits.Where(item => IsSummaryEligible(item.Evaluation)
                    && item.Evaluation.MoneyEstimate?.IsMonetary == true
                    && item.Evaluation.MoneyEstimate.Period == AmountPeriod.Monthly))
        };

        var matcapitalBucket = BuildStrategicBucket(
            "Маткапитал",
            "matcapital",
            evaluatedBenefits.Where(item => IsSummaryEligible(item.Evaluation) && IsMatcapitalBenefit(item.Benefit)));
        if (matcapitalBucket is not null)
        {
            buckets.Add(matcapitalBucket);
        }

        var housingBucket = BuildStrategicBucket(
            "Жильё",
            "housing",
            evaluatedBenefits.Where(item => IsSummaryEligible(item.Evaluation)
                && (item.Evaluation.BenefitType == BenefitType.HousingSupport || item.Benefit.Category == BenefitCategory.Housing)));
        if (housingBucket is not null)
        {
            buckets.Add(housingBucket);
        }

        return buckets;
    }

    private static DashboardSummaryBucketViewModel BuildMoneyBucket(
        string title,
        string semanticKey,
        IEnumerable<EvaluatedDashboardBenefit> source)
    {
        var items = source.ToArray();
        if (items.Length == 0)
        {
            return new DashboardSummaryBucketViewModel(
                title,
                "Сейчас нет подтверждённых сумм",
                "Появится здесь, когда по профилю найдутся подходящие денежные меры этого типа.",
                semanticKey,
                IsMuted: true);
        }

        var exactAmounts = items
            .Select(item => item.Evaluation.MoneyEstimate)
            .OfType<MoneyEstimate>()
            .Where(estimate => estimate.CanParticipateInTotals && estimate.ExactAmount is not null)
            .Select(estimate => estimate.ExactAmount!.Value)
            .ToArray();

        var rangeCount = items.Count(item =>
            item.Evaluation.MoneyEstimate?.CanParticipateInTotals == false
            && item.Evaluation.MoneyEstimate.Kind is MoneyEstimateKind.Range or MoneyEstimateKind.Choice);
        var upperBoundCount = items.Count(item =>
            item.Evaluation.MoneyEstimate?.CanParticipateInTotals == false
            && item.Evaluation.MoneyEstimate.Kind is MoneyEstimateKind.UpTo or MoneyEstimateKind.Percentage);

        var valueText = exactAmounts.Length > 0
            ? MoneyDisplayFormatter.FormatAmount(SumAmounts(exactAmounts))
            : BuildVariableOnlySummary(items);

        var detailParts = new List<string>();
        if (exactAmounts.Length > 0)
        {
            detailParts.Add($"Точных сумм: {exactAmounts.Length}.");
        }

        if (rangeCount > 0)
        {
            detailParts.Add($"Диапазон или выбор по профилю: {rangeCount}.");
        }

        if (upperBoundCount > 0)
        {
            detailParts.Add($"С верхней границей: {upperBoundCount}.");
        }

        if (detailParts.Count == 0)
        {
            detailParts.Add("Сумма показана без смешивания с диапазонами и лимитами.");
        }

        return new DashboardSummaryBucketViewModel(
            title,
            valueText,
            string.Join(" ", detailParts),
            semanticKey,
            IsMuted: false);
    }

    private static DashboardSummaryBucketViewModel? BuildStrategicBucket(
        string title,
        string semanticKey,
        IEnumerable<EvaluatedDashboardBenefit> source)
    {
        var items = source.ToArray();
        if (items.Length == 0)
        {
            return null;
        }

        var primary = items[0];
        var valueText = primary.Evaluation.MoneyEstimate?.DisplayText ?? primary.Benefit.Copy.Title;
        var description = items.Length == 1
            ? primary.Benefit.Copy.ShortDescription
            : $"{primary.Benefit.Copy.Title}. Ещё мер в этом блоке: {items.Length - 1}.";

        return new DashboardSummaryBucketViewModel(
            title,
            valueText,
            description,
            semanticKey,
            IsMuted: false);
    }

    private static string BuildVariableOnlySummary(IReadOnlyList<EvaluatedDashboardBenefit> items)
    {
        if (items.Count == 1)
        {
            return items[0].Evaluation.MoneyEstimate?.DisplayText ?? "Сумма зависит от условий";
        }

        var hasRanges = items.Any(item => item.Evaluation.MoneyEstimate?.Kind is MoneyEstimateKind.Range or MoneyEstimateKind.Choice);
        var hasUpperBounds = items.Any(item => item.Evaluation.MoneyEstimate?.Kind is MoneyEstimateKind.UpTo or MoneyEstimateKind.Percentage);

        if (hasRanges && hasUpperBounds)
        {
            return "Есть суммы с диапазоном и потолком";
        }

        if (hasRanges)
        {
            return "Есть суммы с диапазоном";
        }

        if (hasUpperBounds)
        {
            return "Есть суммы с верхней границей";
        }

        return "Суммы зависят от условий профиля";
    }

    private static MoneyAmount SumAmounts(IReadOnlyList<MoneyAmount> amounts)
    {
        var first = amounts[0];
        return new MoneyAmount(amounts.Sum(amount => amount.Value), first.Currency, first.Period);
    }

    private static bool IsSummaryEligible(EvaluatedBenefit evaluation) =>
        SummaryEligibleStatuses.Contains(evaluation.AvailabilityStatus);

    private static bool IsMatcapitalBenefit(Benefit benefit) =>
        benefit.Slug.Contains("maternity-capital", StringComparison.OrdinalIgnoreCase)
        || benefit.Slug.Contains("mat-capital", StringComparison.OrdinalIgnoreCase);

    private sealed record EvaluatedDashboardBenefit(Benefit Benefit, EvaluatedBenefit Evaluation);
}
