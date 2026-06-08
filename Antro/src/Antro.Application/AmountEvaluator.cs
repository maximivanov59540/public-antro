using Antro.Domain;

namespace Antro.Application;

public sealed class AmountEvaluator : IAmountEvaluator
{
    public MoneyEstimate Evaluate(AmountRule amountRule, UserProfile profile, EvaluationContext context)
    {
        ArgumentNullException.ThrowIfNull(amountRule);
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(context);

        return amountRule switch
        {
            FixedAmount fixedAmount => EvaluateFixedAmount(fixedAmount),
            AmountRange amountRange => EvaluateAmountRange(amountRange),
            UpToAmount upToAmount => EvaluateUpToAmount(upToAmount),
            ChoiceAmount choiceAmount => EvaluateChoiceAmount(choiceAmount, profile),
            PercentageSubsidy percentageSubsidy => EvaluatePercentageSubsidy(percentageSubsidy),
            NaturalSupport naturalSupport => EvaluateNaturalSupport(naturalSupport),
            StatusOnlyAmount statusOnlyAmount => EvaluateStatusOnlyAmount(statusOnlyAmount),
            _ => throw new NotSupportedException($"Unsupported amount rule type: {amountRule.GetType().Name}.")
        };
    }

    private static MoneyEstimate EvaluateFixedAmount(FixedAmount fixedAmount) =>
        new(
            kind: MoneyEstimateKind.Exact,
            period: fixedAmount.Amount.Period,
            displayText: MoneyDisplayFormatter.FormatAmount(fixedAmount.Amount),
            canParticipateInTotals: true,
            exactAmount: fixedAmount.Amount,
            minimumAmount: fixedAmount.Amount,
            maximumAmount: fixedAmount.Amount,
            detailsText: "Точная денежная сумма.");

    private static MoneyEstimate EvaluateAmountRange(AmountRange amountRange) =>
        new(
            kind: MoneyEstimateKind.Range,
            period: amountRange.MinimumAmount.Period,
            displayText: MoneyDisplayFormatter.FormatRange(amountRange.MinimumAmount, amountRange.MaximumAmount),
            canParticipateInTotals: false,
            minimumAmount: amountRange.MinimumAmount,
            maximumAmount: amountRange.MaximumAmount,
            detailsText: "Сумма меняется в пределах показанного диапазона.");

    private static MoneyEstimate EvaluateUpToAmount(UpToAmount upToAmount) =>
        new(
            kind: MoneyEstimateKind.UpTo,
            period: upToAmount.MaximumAmount.Period,
            displayText: MoneyDisplayFormatter.FormatUpTo(upToAmount.MaximumAmount),
            canParticipateInTotals: false,
            maximumAmount: upToAmount.MaximumAmount,
            detailsText: "Показана только верхняя денежная граница.");

    private static MoneyEstimate EvaluateChoiceAmount(ChoiceAmount choiceAmount, UserProfile profile)
    {
        var evaluatedOptions = choiceAmount.Options
            .Select(option => EvaluateOption(option, profile))
            .Where(result => !result.IsExcluded)
            .ToArray();

        if (evaluatedOptions.Length == 0)
        {
            var fallbackOptions = choiceAmount.Options
                .Select(option => new MoneyEstimateOption(option.Amount, option.Label))
                .Distinct()
                .ToArray();

            return BuildChoiceEstimate(
                options: fallbackOptions,
                missingFields: Array.Empty<MissingProfileField>(),
                notes: choiceAmount.Notes ?? "Сумма зависит от дополнительных деталей профиля.");
        }

        var fullyResolved = evaluatedOptions
            .Where(result => result.MissingFields.Count == 0)
            .ToArray();

        if (fullyResolved.Length > 0)
        {
            var highestSpecificity = fullyResolved.Max(result => result.Specificity);
            var bestMatches = fullyResolved
                .Where(result => result.Specificity == highestSpecificity)
                .ToArray();

            if (bestMatches.Length == 1)
            {
                var selected = bestMatches[0].Option;
                return new MoneyEstimate(
                    kind: MoneyEstimateKind.Choice,
                    period: selected.Amount.Period,
                    displayText: MoneyDisplayFormatter.FormatAmount(selected.Amount),
                    canParticipateInTotals: true,
                    exactAmount: selected.Amount,
                    minimumAmount: selected.Amount,
                    maximumAmount: selected.Amount,
                    options:
                    [
                        new MoneyEstimateOption(selected.Amount, selected.Label)
                    ],
                    detailsText: choiceAmount.Notes ?? "Сумма выбрана по уже известным данным профиля.");
            }
        }

        var candidateOptions = evaluatedOptions
            .Select(result => new MoneyEstimateOption(result.Option.Amount, result.Option.Label))
            .Distinct()
            .ToArray();
        var missingFields = evaluatedOptions
            .SelectMany(result => result.MissingFields)
            .Distinct()
            .OrderBy(field => field)
            .ToArray();

        return BuildChoiceEstimate(
            options: candidateOptions,
            missingFields: missingFields,
            notes: choiceAmount.Notes ?? "Точная сумма зависит от дополнительных ответов в профиле.");
    }

    private static MoneyEstimate EvaluatePercentageSubsidy(PercentageSubsidy percentageSubsidy) =>
        new(
            kind: MoneyEstimateKind.Percentage,
            period: percentageSubsidy.CapAmount?.Period ?? AmountPeriod.NotApplicable,
            displayText: MoneyDisplayFormatter.FormatPercentage(percentageSubsidy.Percentage, percentageSubsidy.CapAmount),
            canParticipateInTotals: false,
            percentage: percentageSubsidy.Percentage,
            capAmount: percentageSubsidy.CapAmount,
            detailsText: percentageSubsidy.Notes ?? "Мера выражена в процентах, а не в фиксированной сумме в рублях.");

    private static MoneyEstimate EvaluateNaturalSupport(NaturalSupport naturalSupport) =>
        new(
            kind: MoneyEstimateKind.NaturalSupport,
            period: AmountPeriod.NotApplicable,
            displayText: "\u041d\u0430\u0442\u0443\u0440\u0430\u043b\u044c\u043d\u0430\u044f \u043f\u043e\u043c\u043e\u0449\u044c",
            canParticipateInTotals: false,
            detailsText: naturalSupport.Description);

    private static MoneyEstimate EvaluateStatusOnlyAmount(StatusOnlyAmount statusOnlyAmount) =>
        new(
            kind: MoneyEstimateKind.StatusOnly,
            period: AmountPeriod.NotApplicable,
            displayText: statusOnlyAmount.StatusText,
            canParticipateInTotals: false,
            detailsText: "Неденежное право или статусная мера.");

    private static MoneyEstimate BuildChoiceEstimate(
        IReadOnlyList<MoneyEstimateOption> options,
        IReadOnlyList<MissingProfileField> missingFields,
        string notes)
    {
        if (options.Count == 0)
        {
            return new MoneyEstimate(
                kind: MoneyEstimateKind.Choice,
                period: AmountPeriod.NotApplicable,
                displayText: "\u0421\u0443\u043c\u043c\u0430 \u0437\u0430\u0432\u0438\u0441\u0438\u0442 \u043e\u0442 \u043f\u0440\u043e\u0444\u0438\u043b\u044f",
                canParticipateInTotals: false,
                missingFields: missingFields,
                detailsText: notes);
        }

        var firstAmount = options[0].Amount;
        var sameShape = options.All(option =>
            option.Amount.Currency == firstAmount.Currency &&
            option.Amount.Period == firstAmount.Period);

        if (sameShape)
        {
            var minimumAmount = options.MinBy(option => option.Amount.Value)!.Amount;
            var maximumAmount = options.MaxBy(option => option.Amount.Value)!.Amount;
            var displayText = minimumAmount == maximumAmount
                ? MoneyDisplayFormatter.FormatAmount(minimumAmount)
                : MoneyDisplayFormatter.FormatRange(minimumAmount, maximumAmount);

            return new MoneyEstimate(
                kind: MoneyEstimateKind.Choice,
                period: firstAmount.Period,
                displayText: displayText,
                canParticipateInTotals: false,
                minimumAmount: minimumAmount,
                maximumAmount: maximumAmount,
                options: options,
                missingFields: missingFields,
                detailsText: notes);
        }

        return new MoneyEstimate(
            kind: MoneyEstimateKind.Choice,
            period: AmountPeriod.NotApplicable,
            displayText: "\u0421\u0443\u043c\u043c\u0430 \u0437\u0430\u0432\u0438\u0441\u0438\u0442 \u043e\u0442 \u043f\u0440\u043e\u0444\u0438\u043b\u044f",
            canParticipateInTotals: false,
            options: options,
            missingFields: missingFields,
            detailsText: notes);
    }

    private static EvaluatedChoiceOption EvaluateOption(ChoiceAmountOption option, UserProfile profile)
    {
        var missingFields = new HashSet<MissingProfileField>();
        var specificity = 0;

        if (!MatchesCriterion(option.ChildOrder, profile.ChildOrder, MissingProfileField.ChildOrder, missingFields, ref specificity))
        {
            return EvaluatedChoiceOption.Excluded;
        }

        if (!MatchesCriterion(option.MatCapitalHistory, profile.MatCapitalHistory, MissingProfileField.MatCapitalHistory, missingFields, ref specificity))
        {
            return EvaluatedChoiceOption.Excluded;
        }

        if (!MatchesCriterion(option.FamilyStatus, profile.FamilyStatus, MissingProfileField.FamilyStatus, missingFields, ref specificity))
        {
            return EvaluatedChoiceOption.Excluded;
        }

        if (!MatchesCriterion(option.EmploymentStatus, profile.EmploymentStatus, MissingProfileField.EmploymentStatus, missingFields, ref specificity))
        {
            return EvaluatedChoiceOption.Excluded;
        }

        if (!MatchesCriterion(option.ParentAgeBand, profile.ParentAgeBand, MissingProfileField.ParentAgeBand, missingFields, ref specificity))
        {
            return EvaluatedChoiceOption.Excluded;
        }

        if (!MatchesCriterion(option.IncomeBand, profile.IncomeBand, MissingProfileField.IncomeBand, missingFields, ref specificity))
        {
            return EvaluatedChoiceOption.Excluded;
        }

        if (!MatchesCriterion(option.PropertyStatus, profile.PropertyStatus, MissingProfileField.PropertyStatus, missingFields, ref specificity))
        {
            return EvaluatedChoiceOption.Excluded;
        }

        if (!MatchesCriterion(option.MortgageIntent, profile.MortgageIntent, MissingProfileField.MortgageIntent, missingFields, ref specificity))
        {
            return EvaluatedChoiceOption.Excluded;
        }

        return new EvaluatedChoiceOption(option, specificity, missingFields.ToArray(), IsExcluded: false);
    }

    private static bool MatchesCriterion<T>(
        T? expectedValue,
        T? actualValue,
        MissingProfileField missingField,
        ISet<MissingProfileField> missingFields,
        ref int specificity)
        where T : struct, Enum
    {
        if (expectedValue is null)
        {
            return true;
        }

        specificity++;

        if (actualValue is null)
        {
            missingFields.Add(missingField);
            return true;
        }

        return EqualityComparer<T>.Default.Equals(expectedValue.Value, actualValue.Value);
    }

    private sealed record EvaluatedChoiceOption(
        ChoiceAmountOption Option,
        int Specificity,
        IReadOnlyList<MissingProfileField> MissingFields,
        bool IsExcluded)
    {
        public static EvaluatedChoiceOption Excluded { get; } =
            new(new ChoiceAmountOption(new MoneyAmount(0m, Currency.Rub, AmountPeriod.NotApplicable)), 0, Array.Empty<MissingProfileField>(), true);
    }
}
