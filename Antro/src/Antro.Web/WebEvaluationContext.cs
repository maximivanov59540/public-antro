using Antro.Content.Moscow2026;
using Antro.Domain;

namespace Antro.Web;

internal static class WebEvaluationContext
{
    public static EvaluationContext CreateMoscow2026(DateOnly today) =>
        new(
            Today: today,
            Region: Moscow2026BenefitCatalog.Region,
            CatalogVersion: Moscow2026BenefitCatalog.CatalogVersion,
            Assumptions: new AssumedMvpEligibilityContext(true, true, true));
}
