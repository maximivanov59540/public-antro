using Antro.Domain;

namespace Antro.Web;

internal sealed record StageRouteItem(
    string Slug,
    LifeStage Stage,
    string Title,
    string Subtitle,
    string RangeLabel);

internal static class StageNavigation
{
    public static IReadOnlyList<StageRouteItem> All { get; } =
    [
        new("expecting",            LifeStage.ExpectingChild,           "Ждём ребёнка",    "Беременность",       "Беременность"),
        new("newborn",              LifeStage.NewbornZeroToTwoMonths,   "Только родился",  "0–2 месяца",         "0–2 месяца"),
        new("up-to-six-months",     LifeStage.UpToSixMonths,            "До полугода",     "2–6 месяцев",        "2–6 месяцев"),
        new("up-to-eighteen-months",LifeStage.UpToEighteenMonths,       "До полутора лет", "6 месяцев – 1,5 года","6 месяцев – 1,5 года"),
        new("up-to-three-years",    LifeStage.UpToThreeYears,           "До трёх",         "1,5–3 года",         "1,5–3 года"),
        new("older-than-three-years",LifeStage.OlderThanThreeYears,     "Старше трёх",     "3+",                 "3+"),
    ];

    public static bool TryGetBySlug(string? slug, out StageRouteItem? item)
    {
        item = All.FirstOrDefault(candidate => string.Equals(candidate.Slug, slug, StringComparison.OrdinalIgnoreCase));
        return item is not null;
    }

    public static StageRouteItem GetByStage(LifeStage stage) =>
        All.First(item => item.Stage == stage);
}
