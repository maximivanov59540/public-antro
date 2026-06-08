using Antro.Domain;

namespace Antro.Content.Moscow2026;

internal static class Moscow2026PriorityHints
{
    public static PriorityHint NewbornGiftSet { get; } = new(hiddenGemScore: 3, actionabilityBoost: 3);

    public static PriorityHint BirthDeadlinePayment { get; } = new(hiddenGemScore: 1, actionabilityBoost: 3);

    public static PriorityHint YoungFamilyDeadlinePayment { get; } = new(hiddenGemScore: 2, actionabilityBoost: 2);

    public static PriorityHint HiddenTaxRoute { get; } = new(hiddenGemScore: 2, actionabilityBoost: 1);

    public static PriorityHint HousingRecommendation { get; } = new(hiddenGemScore: 1, actionabilityBoost: 1);
}
