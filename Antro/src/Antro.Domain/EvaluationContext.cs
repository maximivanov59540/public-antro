namespace Antro.Domain;

public sealed record AssumedMvpEligibilityContext(
    bool AssumesRussianCitizenship,
    bool AssumesMoscowRegistration,
    bool AssumesMoscow2026Rules);

public sealed record EvaluationContext(
    DateOnly Today,
    RegionCode Region,
    CatalogVersion CatalogVersion,
    AssumedMvpEligibilityContext Assumptions);
