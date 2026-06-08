using Antro.Domain;

namespace Antro.Application;

public sealed class DeadlineEvaluator : IDeadlineEvaluator
{
    private const int UrgentThresholdDays = 7;
    private const int SoonThresholdDays = 30;

    public DeadlineEvaluation Evaluate(DeadlineRule? deadlineRule, UserProfile profile, EvaluationContext context)
    {
        if (deadlineRule is null)
        {
            return new DeadlineEvaluation(
                DeadlineStatus.None,
                DeadlineKind.None,
                null,
                null,
                "Без отдельного срока",
                "Для этой меры в MVP не задан отдельный срок подачи.");
        }

        return deadlineRule switch
        {
            NoDeadline noDeadline => new DeadlineEvaluation(
                DeadlineStatus.None,
                DeadlineKind.None,
                null,
                null,
                "Без отдельного срока",
                noDeadline.Notes ?? "Для этой меры нет обычного отдельного срока подачи."),
            FilingDeadlineFromBirth filingDeadlineFromBirth => EvaluateFilingDeadlineFromBirth(
                filingDeadlineFromBirth,
                profile,
                context.Today),
            ActiveUntilChildAge activeUntilChildAge => EvaluateActiveUntilChildAge(
                activeUntilChildAge,
                profile,
                context.Today),
            FixedPolicyEndDate fixedPolicyEndDate => EvaluateFixedPolicyEndDate(
                fixedPolicyEndDate,
                context.Today),
            _ => new DeadlineEvaluation(
                DeadlineStatus.Unknown,
                DeadlineKind.None,
                null,
                null,
                "Срок не определён",
                "Текущий MVP-оценщик не распознаёт этот тип правила по сроку.")
        };
    }

    private static DeadlineEvaluation EvaluateFilingDeadlineFromBirth(
        FilingDeadlineFromBirth deadlineRule,
        UserProfile profile,
        DateOnly today)
    {
        if (profile.ChildDate is null || profile.DateInputKind != DateInputKind.BirthDate)
        {
            return new DeadlineEvaluation(
                DeadlineStatus.Unknown,
                DeadlineKind.FilingDeadline,
                null,
                null,
                "Срок подачи не определён",
                "Чтобы рассчитать этот срок подачи, нужна подтверждённая дата рождения ребёнка.");
        }

        // MVP behavior is inclusive: filing remains active on the due date itself
        // and becomes expired starting the following day.
        var deadlineDate = profile.ChildDate.Value.AddMonths(deadlineRule.MonthsFromBirth);
        var daysLeft = deadlineDate.DayNumber - today.DayNumber;
        var status = ResolveStatus(daysLeft);

        return new DeadlineEvaluation(
            status,
            DeadlineKind.FilingDeadline,
            deadlineDate,
            daysLeft,
            BuildShortText("Подать до", status, deadlineDate, daysLeft),
            deadlineRule.Notes ?? BuildExplanation(
                status,
                deadlineDate,
                daysLeft,
                $"Срок подачи считается как дата рождения плюс {deadlineRule.MonthsFromBirth} мес., включая саму граничную дату."));
    }

    private static DeadlineEvaluation EvaluateActiveUntilChildAge(
        ActiveUntilChildAge deadlineRule,
        UserProfile profile,
        DateOnly today)
    {
        if (profile.ChildDate is null || profile.DateInputKind != DateInputKind.BirthDate)
        {
            return new DeadlineEvaluation(
                DeadlineStatus.Unknown,
                DeadlineKind.ActiveUntil,
                null,
                null,
                "Граница действия не определена",
                "Чтобы рассчитать эту границу действия, нужна подтверждённая дата рождения ребёнка.");
        }

        var boundaryDate = profile.ChildDate.Value;

        if (deadlineRule.Years is not null)
        {
            boundaryDate = boundaryDate.AddYears(deadlineRule.Years.Value);
        }

        if (deadlineRule.Months is not null)
        {
            boundaryDate = boundaryDate.AddMonths(deadlineRule.Months.Value);
        }

        // MVP behavior is inclusive: the benefit stays active on the boundary date
        // itself and becomes expired starting the following day.
        var daysLeft = boundaryDate.DayNumber - today.DayNumber;
        var status = ResolveStatus(daysLeft);

        return new DeadlineEvaluation(
            status,
            DeadlineKind.ActiveUntil,
            boundaryDate,
            daysLeft,
            BuildShortText("Действует до", status, boundaryDate, daysLeft),
            deadlineRule.Notes ?? BuildExplanation(
                status,
                boundaryDate,
                daysLeft,
                $"Граница действия считается от даты рождения с учётом настроенного смещения по месяцам или годам, включая саму граничную дату."));
    }

    private static DeadlineEvaluation EvaluateFixedPolicyEndDate(FixedPolicyEndDate deadlineRule, DateOnly today)
    {
        var daysLeft = deadlineRule.EndDate.DayNumber - today.DayNumber;
        var status = ResolveStatus(daysLeft);

        return new DeadlineEvaluation(
            status,
            DeadlineKind.FixedPolicyEndDate,
            deadlineRule.EndDate,
            daysLeft,
            BuildShortText("Программа действует до", status, deadlineRule.EndDate, daysLeft),
            deadlineRule.Notes ?? BuildExplanation(
                status,
                deadlineRule.EndDate,
                daysLeft,
                "Программа считается действующей по саму фиксированную дату и истекает на следующий день."));
    }

    private static DeadlineStatus ResolveStatus(int daysLeft) =>
        daysLeft switch
        {
            < 0 => DeadlineStatus.Expired,
            <= UrgentThresholdDays => DeadlineStatus.Urgent,
            <= SoonThresholdDays => DeadlineStatus.Soon,
            _ => DeadlineStatus.Active
        };

    private static string BuildShortText(string prefix, DeadlineStatus status, DateOnly deadlineDate, int daysLeft) =>
        status switch
        {
            DeadlineStatus.Expired => $"{prefix}: срок прошёл {deadlineDate:yyyy-MM-dd}",
            DeadlineStatus.Urgent => $"{prefix}: осталось {daysLeft} дн.",
            DeadlineStatus.Soon => $"{prefix}: осталось {daysLeft} дн.",
            DeadlineStatus.Active => $"{prefix}: действует до {deadlineDate:yyyy-MM-dd}",
            DeadlineStatus.None => "Без отдельного срока",
            _ => $"{prefix}: нужно уточнить"
        };

    private static string BuildExplanation(DeadlineStatus status, DateOnly deadlineDate, int daysLeft, string ruleExplanation) =>
        status switch
        {
            DeadlineStatus.Expired => $"{ruleExplanation} Граничная дата была {deadlineDate:yyyy-MM-dd}.",
            DeadlineStatus.Urgent => $"{ruleExplanation} Осталось {daysLeft} дн., включая дату {deadlineDate:yyyy-MM-dd}.",
            DeadlineStatus.Soon => $"{ruleExplanation} До даты {deadlineDate:yyyy-MM-dd} осталось {daysLeft} дн.",
            DeadlineStatus.Active => $"{ruleExplanation} Текущая граничная дата: {deadlineDate:yyyy-MM-dd}.",
            _ => ruleExplanation
        };
}
