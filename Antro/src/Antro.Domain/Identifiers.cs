namespace Antro.Domain;

public readonly record struct BenefitId
{
    public BenefitId(string value)
    {
        Value = DomainGuard.RequiredText(value, nameof(value));
    }

    public string Value { get; }

    public override string ToString() => Value;
}

public readonly record struct RegionCode
{
    public RegionCode(string value)
    {
        Value = DomainGuard.RequiredText(value, nameof(value));
    }

    public string Value { get; }

    public override string ToString() => Value;
}

public readonly record struct CatalogVersion
{
    public CatalogVersion(string value)
    {
        Value = DomainGuard.RequiredText(value, nameof(value));
    }

    public string Value { get; }

    public override string ToString() => Value;
}
