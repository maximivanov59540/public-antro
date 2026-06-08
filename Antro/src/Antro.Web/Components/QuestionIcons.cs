namespace Antro.Web.Components;

public static class QuestionIcons
{
    private static readonly IReadOnlyDictionary<string, string> Icons =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["rings"] = """<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="9" cy="14" r="5"/><circle cx="15" cy="14" r="5"/><path d="M7 6l2 3"/><path d="M17 6l-2 3"/></svg>""",
            ["pair"] = """<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="8" cy="7" r="2.4"/><circle cx="16" cy="7" r="2.4"/><path d="M3 20c.6-3 2.6-5 5-5s4.4 2 5 5"/><path d="M11 20c.6-3 2.6-5 5-5s4.4 2 5 5"/></svg>""",
            ["person"] = """<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="7" r="3"/><path d="M5 21c.8-4 3.5-6 7-6s6.2 2 7 6"/></svg>""",
            ["female"] = """<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="8" r="4.5"/><path d="M12 12.5v8.5"/><path d="M9 18h6"/></svg>""",
            ["male"] = """<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="10" cy="14" r="5.5"/><path d="m14 10 6-6"/><path d="M15 4h5v5"/></svg>""",
            ["neutral"] = """<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="9"/><path d="M9 14h6"/></svg>""",
            ["check"] = """<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M20 6 9 17l-5-5"/></svg>""",
            ["q"] = """<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="9"/><path d="M9.3 9.5a2.7 2.7 0 0 1 5.2 1c0 1.8-2.5 1.8-2.5 3.5"/><circle cx="12" cy="17.5" r=".7" fill="currentColor"/></svg>""",
            ["house"] = """<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="m3 11 9-7 9 7"/><path d="M5 10v10h14V10"/><path d="M10 20v-6h4v6"/></svg>""",
            ["coins"] = """<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><ellipse cx="9" cy="7" rx="6" ry="3"/><path d="M3 7v4c0 1.66 2.69 3 6 3s6-1.34 6-3V7"/><path d="M3 11v4c0 1.66 2.69 3 6 3"/><ellipse cx="15" cy="14" rx="6" ry="3"/><path d="M9 17v.5c0 1.66 2.69 3 6 3s6-1.34 6-3V14"/></svg>""",
            ["nope"] = """<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="9"/><path d="m9 9 6 6"/><path d="m15 9-6 6"/></svg>""",
        };

    private const string Fallback = """<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="9"/></svg>""";

    public static string Get(string? name) =>
        name is not null && Icons.TryGetValue(name, out var svg) ? svg : Fallback;
}
