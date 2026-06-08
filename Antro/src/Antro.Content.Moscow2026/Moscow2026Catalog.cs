using Antro.Domain;

namespace Antro.Content.Moscow2026;

public static class Moscow2026Catalog
{
    public static RegionCode Region => Moscow2026BenefitCatalog.Region;

    public static CatalogVersion CatalogVersion => Moscow2026BenefitCatalog.CatalogVersion;

    public static Moscow2026Thresholds Thresholds => Moscow2026BenefitCatalog.Thresholds;

    public static IReadOnlyList<Benefit> GetBenefits() => Moscow2026BenefitCatalog.All;
}
