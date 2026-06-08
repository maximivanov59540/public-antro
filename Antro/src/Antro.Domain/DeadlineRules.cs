namespace Antro.Domain;

public abstract record DeadlineRule;

public sealed record FilingDeadlineFromBirth : DeadlineRule
{
    public FilingDeadlineFromBirth(int monthsFromBirth, string? notes = null)
    {
        if (monthsFromBirth < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(monthsFromBirth), "Months from birth cannot be negative.");
        }

        MonthsFromBirth = monthsFromBirth;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    public int MonthsFromBirth { get; }

    public string? Notes { get; }
}

public sealed record ActiveUntilChildAge : DeadlineRule
{
    public ActiveUntilChildAge(int? months, int? years, string? notes = null)
    {
        if (months is null && years is null)
        {
            throw new ArgumentException("Either months or years must be provided.", nameof(months));
        }

        if (months < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(months), "Child age in months cannot be negative.");
        }

        if (years < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(years), "Child age in years cannot be negative.");
        }

        Months = months;
        Years = years;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    public ActiveUntilChildAge(int childAgeInMonths, string? notes = null)
        : this(childAgeInMonths, null, notes)
    {
    }

    public int? Months { get; }

    public int? Years { get; }

    public string? Notes { get; }
}

public sealed record FixedPolicyEndDate : DeadlineRule
{
    public FixedPolicyEndDate(DateOnly endDate, string? notes)
    {
        EndDate = endDate;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    public DateOnly EndDate { get; }

    public string? Notes { get; }
}

public sealed record NoDeadline : DeadlineRule
{
    public NoDeadline(string? notes)
    {
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    public string? Notes { get; }
}
