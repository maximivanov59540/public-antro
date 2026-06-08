using Antro.Domain;

namespace Antro.Application;

public static class DemoProfiles
{
    public static UserProfile MariaNewborn(DateOnly today) =>
        new()
        {
            // Demo script: "married / both officially employed / both under 36 /
            // ordinary housing / planning a mortgage" — expressed directly through
            // the household-level profile enums below.
            ChildDate = today.AddDays(-14),
            DateInputKind = DateInputKind.BirthDate,
            ChildOrder = ChildOrder.First,
            MatCapitalHistory = MatCapitalHistory.NeverReceived,
            FamilyStatus = FamilyStatus.Married,
            EmploymentStatus = EmploymentStatus.Both,
            ParentAgeBand = ParentAgeBand.BothUnderThirtySix,
            IncomeBand = IncomeBand.UpToOneLivingMinimum,
            PropertyStatus = PropertyStatus.OwnsHome,
            MortgageIntent = MortgageIntent.PlansToUseMortgage
        };
}
