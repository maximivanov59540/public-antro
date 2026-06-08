namespace Antro.Domain;

public static class ChildLifeStageCalculator
{
    public static LifeStage Classify(DateOnly childDate, DateInputKind dateInputKind, DateOnly today) =>
        dateInputKind switch
        {
            DateInputKind.Unknown => throw new ArgumentException(
                "Date input kind must be known before classifying a life stage.",
                nameof(dateInputKind)),
            DateInputKind.BirthDate => ClassifyBirthDate(childDate, today),
            DateInputKind.DueDate => ClassifyExpectedChildDate(childDate, today),
            _ => throw new ArgumentOutOfRangeException(nameof(dateInputKind), dateInputKind, "Unsupported date input kind.")
        };

    public static LifeStage ClassifyBirthDate(DateOnly birthDate, DateOnly today)
    {
        if (birthDate > today)
        {
            throw new ArgumentOutOfRangeException(nameof(birthDate), "Birth date cannot be in the future relative to today.");
        }

        if (today < birthDate.AddMonths(2))
        {
            return LifeStage.NewbornZeroToTwoMonths;
        }

        if (today < birthDate.AddMonths(6))
        {
            return LifeStage.UpToSixMonths;
        }

        if (today < birthDate.AddMonths(18))
        {
            return LifeStage.UpToEighteenMonths;
        }

        if (today <= birthDate.AddYears(3))
        {
            return LifeStage.UpToThreeYears;
        }

        return LifeStage.OlderThanThreeYears;
    }

    public static LifeStage ClassifyExpectedChildDate(DateOnly expectedChildDate, DateOnly today)
    {
        if (expectedChildDate < today)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedChildDate),
                "Expected child date cannot be in the past relative to today.");
        }

        return LifeStage.ExpectingChild;
    }
}
