# Antro Implementation Rules

These constraints are non-negotiable for the MVP foundation and all subsequent implementation steps.

1. Antro MVP is a static client-only Blazor WebAssembly application.
2. No backend, no database, no EF Core, no ASP.NET Identity, no controller APIs, no authentication.
3. User questionnaire answers live only in memory by default.
4. Do not use localStorage, cookies, analytics, trackers, or external data submission unless explicitly requested later.
5. UI must not decide eligibility, deadlines, amounts, or sorting.
6. UI must consume view models built by the application layer.
7. The benefit catalog must be typed content, not arrays hardcoded inside Razor components.
8. All 31 Moscow 2026 benefits will later exist as typed entries in `Antro.Content.Moscow2026`.
9. Deadlines and amounts must later be calculated by dedicated domain/application logic, not handwritten UI strings.
10. Do not port the existing HTML prototypes as monolithic Razor pages.
11. Do not add admin panels, CMS, notifications, AI chatbot features, integrations with mos.ru/Gosuslugi/SFR, or all-Russia coverage.
12. Keep the MVP desktop-first, but do not intentionally block future mobile adaptation.
13. The demo route `/demo/maria` will later use the same evaluator and builders as the normal flow; it must not bypass architecture.
14. Every implementation step must keep `dotnet restore`, `dotnet build`, and `dotnet test` green.
