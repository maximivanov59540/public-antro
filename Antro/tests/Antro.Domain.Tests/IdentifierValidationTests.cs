using Antro.Domain;

namespace Antro.Domain.Tests;

public sealed class IdentifierValidationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void BenefitId_rejects_blank_values(string? value)
    {
        Assert.Throws<ArgumentException>(() => new BenefitId(value!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void RegionCode_rejects_blank_values(string? value)
    {
        Assert.Throws<ArgumentException>(() => new RegionCode(value!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void CatalogVersion_rejects_blank_values(string? value)
    {
        Assert.Throws<ArgumentException>(() => new CatalogVersion(value!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Currency_rejects_blank_values(string? value)
    {
        Assert.Throws<ArgumentException>(() => new Currency(value!));
    }
}
