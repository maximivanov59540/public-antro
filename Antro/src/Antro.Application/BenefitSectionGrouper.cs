namespace Antro.Application;

internal static class BenefitSectionGrouper
{
    private static readonly string[] SectionOrder = ["urgent", "rights", "other"];

    public static IReadOnlyList<BenefitSectionViewModel> Group(IReadOnlyList<BenefitCardViewModel> cards)
    {
        var grouped = cards
            .GroupBy(c => c.SectionCategoryKey)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<BenefitCardViewModel>)g.ToArray());

        return SectionOrder
            .Where(key => grouped.ContainsKey(key))
            .Select(key => new BenefitSectionViewModel(
                SectionKey:          key,
                SectionHeaderText:   ApplicationDisplayText.GetSectionHeaderText(key),
                SectionColorClass:   ApplicationDisplayText.GetSectionColorClass(key),
                SectionSubtitleText: ApplicationDisplayText.GetSectionSubtitleText(key),
                Cards:               grouped[key]))
            .ToArray();
    }
}
