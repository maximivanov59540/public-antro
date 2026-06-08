namespace Antro.Domain;

public enum DateInputKind
{
    Unknown = 0,
    BirthDate = 1,
    DueDate = 2
}

public enum ChildOrder
{
    First = 0,
    Second = 1,
    ThirdOrLater = 2
}

public enum MatCapitalHistory
{
    NeverReceived = 0,
    ReceivedUnused = 1,
    PartiallyUsed = 2,
    FullyUsed = 3
}

public enum FamilyStatus
{
    SingleParent = 0,
    Married = 1,
    Partners = 2
}

public enum EmploymentStatus
{
    MotherOnly = 0,
    FatherOnly = 1,
    Both = 2,
    NeitherParent = 3
}

public enum ParentAgeBand
{
    BothUnderThirtySix = 0,
    AtLeastOneThirtySixOrOlder = 1,
    Mixed = 2
}

public enum IncomeBand
{
    Unknown = 0,
    UpToOneLivingMinimum = 1,
    UpToOneAndHalfLivingMinimum = 2,
    UpToTwoLivingMinimums = 3,
    AboveTwoLivingMinimums = 4
}

public enum PropertyStatus
{
    DoesNotOwnHome = 0,
    OwnsHome = 1,
    Unsure = 2
}

public enum MortgageIntent
{
    None = 0,
    PlansToUseMortgage = 1,
    HasActiveMortgage = 2
}

public sealed record UserProfile
{
    public DateOnly? ChildDate { get; init; }

    public DateInputKind DateInputKind { get; init; } = DateInputKind.Unknown;

    public ChildOrder? ChildOrder { get; init; }

    public MatCapitalHistory? MatCapitalHistory { get; init; }

    public FamilyStatus? FamilyStatus { get; init; }

    public EmploymentStatus? EmploymentStatus { get; init; }

    public ParentAgeBand? ParentAgeBand { get; init; }

    public IncomeBand? IncomeBand { get; init; }

    public PropertyStatus? PropertyStatus { get; init; }

    public MortgageIntent? MortgageIntent { get; init; }
}
