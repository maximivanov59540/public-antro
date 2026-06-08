using Antro.Domain;

namespace Antro.Application;

public sealed class BenefitEvaluator : IBenefitEvaluator
{
    private readonly IDeadlineEvaluator deadlineEvaluator;
    private readonly IAmountEvaluator amountEvaluator;
    private readonly IBenefitPrioritySorter prioritySorter;

    public BenefitEvaluator(
        IDeadlineEvaluator? deadlineEvaluator = null,
        IAmountEvaluator? amountEvaluator = null,
        IBenefitPrioritySorter? prioritySorter = null)
    {
        this.deadlineEvaluator = deadlineEvaluator ?? new DeadlineEvaluator();
        this.amountEvaluator = amountEvaluator ?? new AmountEvaluator();
        this.prioritySorter = prioritySorter ?? new BenefitPrioritySorter();
    }

    public IReadOnlyList<EvaluatedBenefit> Evaluate(
        IReadOnlyList<Benefit> benefits,
        UserProfile profile,
        EvaluationContext context)
    {
        if (benefits is null)
        {
            throw new ArgumentNullException(nameof(benefits));
        }

        if (profile is null)
        {
            throw new ArgumentNullException(nameof(profile));
        }

        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var evaluatedBenefits = benefits
            .Select(benefit => EvaluateBenefit(benefit, profile, context))
            .ToArray();

        return prioritySorter.Sort(evaluatedBenefits);
    }

    private EvaluatedBenefit EvaluateBenefit(Benefit benefit, UserProfile profile, EvaluationContext context)
    {
        var ruleResults = benefit.EligibilityRules
            .Select(rule => rule.Evaluate(profile, context))
            .ToArray();

        var missingFields = ruleResults
            .SelectMany(result => result.MissingFields)
            .Distinct()
            .ToArray();

        var deadline = deadlineEvaluator.Evaluate(benefit.DeadlineRule, profile, context);
        var moneyEstimate = amountEvaluator.Evaluate(benefit.AmountRule, profile, context);
        var evaluationMissingFields = missingFields
            .Concat(moneyEstimate.MissingFields)
            .Distinct()
            .ToArray();
        var stageAvailability = ResolveCurrentStageAvailability(benefit, profile, context);

        var failResult = ruleResults.FirstOrDefault(result => result.Outcome == RuleOutcome.Fail);
        if (failResult is not null)
        {
            return CreateEvaluatedBenefit(
                benefit,
                AvailabilityStatus.Unavailable,
                deadline,
                moneyEstimate,
                failResult.Explanation ?? "Одно или несколько условий для этой меры сейчас не выполняются.",
                evaluationMissingFields,
                stageAvailability);
        }

        var unknownResults = ruleResults
            .Where(result => result.Outcome == RuleOutcome.Unknown)
            .ToArray();

        if (unknownResults.Length > 0)
        {
            var unknownReason = unknownResults.FirstOrDefault(result => !string.IsNullOrWhiteSpace(result.Explanation))?.Explanation
                ?? "Для оценки этой меры нужны дополнительные ответы в профиле.";

            return CreateEvaluatedBenefit(
                benefit,
                AvailabilityStatus.NeedsMoreInfo,
                deadline,
                moneyEstimate,
                unknownReason,
                evaluationMissingFields,
                stageAvailability);
        }

        var stageBasedStatus = ResolveStageBasedAvailability(stageAvailability);
        if (stageBasedStatus is not null)
        {
            return CreateEvaluatedBenefit(
                benefit,
                stageBasedStatus.Status,
                deadline,
                moneyEstimate,
                stageBasedStatus.Reason,
                evaluationMissingFields,
                stageAvailability);
        }

        if (deadline.Status == DeadlineStatus.Expired)
        {
            return CreateEvaluatedBenefit(
                benefit,
                AvailabilityStatus.Expired,
                deadline,
                moneyEstimate,
                deadline.Explanation,
                evaluationMissingFields,
                stageAvailability);
        }

        var availabilityStatus = benefit.Type switch
        {
            BenefitType.Automatic => AvailabilityStatus.Automatic,
            BenefitType.InformationalRight => AvailabilityStatus.Informational,
            _ => AvailabilityStatus.Available
        };

        var reason = availabilityStatus switch
        {
            AvailabilityStatus.Automatic => "В каталоге MVP эта мера считается автоматической.",
            AvailabilityStatus.Informational => "Эта карточка носит справочный характер и не требует полной проверки права.",
            _ => "Текущие правила MVP не показывают явных ограничений для этой меры."
        };

        return CreateEvaluatedBenefit(
            benefit,
            availabilityStatus,
            deadline,
            moneyEstimate,
            reason,
            evaluationMissingFields,
            stageAvailability);
    }

    private static CurrentStageAvailability? ResolveCurrentStageAvailability(Benefit benefit, UserProfile profile, EvaluationContext context)
    {
        if (profile.ChildDate is null || profile.DateInputKind == DateInputKind.Unknown)
        {
            return null;
        }

        try
        {
            var lifeStage = ChildLifeStageCalculator.Classify(profile.ChildDate.Value, profile.DateInputKind, context.Today);
            var stageAvailability = benefit.StageAvailability.FirstOrDefault(stage => stage.LifeStage == lifeStage);
            if (stageAvailability is not null)
            {
                return new CurrentStageAvailability(lifeStage, stageAvailability);
            }

            return InferNearestStageAvailability(lifeStage, benefit.StageAvailability);
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    private static CurrentStageAvailability? InferNearestStageAvailability(
        LifeStage currentLifeStage,
        IReadOnlyList<LifeStageAvailability> stageAvailability)
    {
        if (stageAvailability.Count == 0)
        {
            return null;
        }

        var orderedStages = stageAvailability
            .OrderBy(stage => (int)stage.LifeStage)
            .ToArray();
        var earliestStage = orderedStages[0];
        var latestStage = orderedStages[^1];

        if ((int)currentLifeStage < (int)earliestStage.LifeStage)
        {
            return new CurrentStageAvailability(
                currentLifeStage,
                new LifeStageAvailability(
                    currentLifeStage,
                    earliestStage.IsVisible,
                    StageAvailabilityStatus.Future,
                    earliestStage.Explanation ?? "Эта мера обычно становится актуальной позже."));
        }

        if ((int)currentLifeStage > (int)latestStage.LifeStage)
        {
            var inferredStatus = latestStage.Status == StageAvailabilityStatus.Informational
                ? StageAvailabilityStatus.Informational
                : StageAvailabilityStatus.Expired;

            return new CurrentStageAvailability(
                currentLifeStage,
                new LifeStageAvailability(
                    currentLifeStage,
                    latestStage.IsVisible,
                    inferredStatus,
                    latestStage.Explanation ?? "Эта мера обычно относилась к более раннему этапу семьи."));
        }

        return null;
    }

    private static StageBasedAvailability? ResolveStageBasedAvailability(CurrentStageAvailability? stageAvailability)
    {
        if (stageAvailability is null)
        {
            return null;
        }

        return stageAvailability.Availability.Status switch
        {
            StageAvailabilityStatus.Active => null,
            StageAvailabilityStatus.Expired => new StageBasedAvailability(
                AvailabilityStatus.Expired,
                stageAvailability.Availability.Explanation ?? "Для текущего этапа семьи эта мера считается уже завершённой."),
            StageAvailabilityStatus.Future => new StageBasedAvailability(
                AvailabilityStatus.PotentiallyAvailable,
                stageAvailability.Availability.Explanation ?? "Эта мера обычно становится актуальной на более позднем этапе."),
            StageAvailabilityStatus.Informational => new StageBasedAvailability(
                AvailabilityStatus.Informational,
                stageAvailability.Availability.Explanation ?? "Эта мера показана для справки на текущем этапе семьи."),
            StageAvailabilityStatus.StageDependent => new StageBasedAvailability(
                AvailabilityStatus.PotentiallyAvailable,
                stageAvailability.Availability.Explanation ?? "Доступность этой меры зависит от более точных условий внутри этапа."),
            _ => null
        };
    }

    private static EvaluatedBenefit CreateEvaluatedBenefit(
        Benefit benefit,
        AvailabilityStatus availabilityStatus,
        DeadlineEvaluation deadline,
        MoneyEstimate? moneyEstimate,
        string reason,
        IReadOnlyList<MissingProfileField> missingFields,
        CurrentStageAvailability? currentStageAvailability)
    {
        var badges = CreateBadges(availabilityStatus, deadline, missingFields, benefit, currentStageAvailability);
        return new EvaluatedBenefit(
            benefit.Id,
            benefit.Slug,
            benefit.Type,
            benefit.Tier,
            availabilityStatus,
            deadline,
            moneyEstimate,
            priorityScore: 0,
            userFacingReason: reason,
            missingFields,
            badges,
            benefit.PriorityHint);
    }

    private static IReadOnlyList<DisplayBadge> CreateBadges(
        AvailabilityStatus availabilityStatus,
        DeadlineEvaluation deadline,
        IReadOnlyList<MissingProfileField> missingFields,
        Benefit benefit,
        CurrentStageAvailability? currentStageAvailability)
    {
        var badges = new List<DisplayBadge>
        {
            new(DisplayBadgeKind.Status, availabilityStatus.ToString(), $"status-{availabilityStatus.ToString().ToLowerInvariant()}")
        };

        if (deadline.Status is DeadlineStatus.Active or DeadlineStatus.Soon or DeadlineStatus.Urgent or DeadlineStatus.Expired or DeadlineStatus.None)
        {
            badges.Add(new DisplayBadge(
                deadline.Status is DeadlineStatus.Urgent or DeadlineStatus.Expired ? DisplayBadgeKind.Urgency : DisplayBadgeKind.Deadline,
                deadline.ShortText,
                $"deadline-{deadline.Status.ToString().ToLowerInvariant()}"));
        }

        if (availabilityStatus == AvailabilityStatus.Automatic)
        {
            badges.Add(new DisplayBadge(DisplayBadgeKind.Automatic, "Автоматически", "automatic"));
        }

        if (availabilityStatus == AvailabilityStatus.Informational)
        {
            badges.Add(new DisplayBadge(DisplayBadgeKind.Informational, "Справочно", "informational"));
        }

        if (missingFields.Count > 0)
        {
            badges.Add(new DisplayBadge(DisplayBadgeKind.MissingInfo, "Нужны данные профиля", "missing-info"));
        }

        if (benefit.Type == BenefitType.IncomeDependentPayment)
        {
            badges.Add(new DisplayBadge(DisplayBadgeKind.Income, "Зависит от дохода", "income-related"));
        }

        if (currentStageAvailability?.Availability.Status == StageAvailabilityStatus.Future)
        {
            badges.Add(new DisplayBadge(DisplayBadgeKind.Urgency, "Позже по этапу", "future-stage"));
        }

        if (deadline.Status == DeadlineStatus.Expired && availabilityStatus != AvailabilityStatus.Unavailable)
        {
            badges.Add(new DisplayBadge(DisplayBadgeKind.Urgency, "Срок прошёл", "deadline-passed"));
        }

        return badges;
    }
    private sealed record CurrentStageAvailability(LifeStage LifeStage, LifeStageAvailability Availability);

    private sealed record StageBasedAvailability(AvailabilityStatus Status, string Reason);
}
