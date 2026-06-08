using Antro.Domain;

namespace Antro.Domain.Tests;

public sealed class UserProfileTests
{
    [Fact]
    public void UserProfile_can_represent_unknown_income_and_incomplete_answers()
    {
        var profile = new UserProfile
        {
            DateInputKind = DateInputKind.Unknown,
            IncomeBand = Antro.Domain.IncomeBand.Unknown
        };

        Assert.Null(profile.ChildDate);
        Assert.Equal(DateInputKind.Unknown, profile.DateInputKind);
        Assert.Equal(Antro.Domain.IncomeBand.Unknown, profile.IncomeBand);
        Assert.Null(profile.ChildOrder);
        Assert.Null(profile.MatCapitalHistory);
        Assert.Null(profile.FamilyStatus);
        Assert.Null(profile.EmploymentStatus);
        Assert.Null(profile.ParentAgeBand);
        Assert.Null(profile.PropertyStatus);
        Assert.Null(profile.MortgageIntent);
    }
}
