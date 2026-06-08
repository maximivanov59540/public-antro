# Antro

Antro is a client-only educational Blazor WebAssembly MVP for demonstrating a benefits navigator for young families in Moscow.

This repository is intentionally limited to a static, no-backend foundation. It does not include a database, accounts, authentication, server-side processing, or persistence by default.

## Project Structure

- `src/Antro.Domain` contains pure domain placeholders and will later hold the core benefit model and evaluation result types.
- `src/Antro.Application` contains application-layer placeholders and will later orchestrate evaluation and view-model building.
- `src/Antro.Content.Moscow2026` contains typed content placeholders and will later hold the Moscow 2026 benefit catalog.
- `src/Antro.Web` contains the standalone Blazor WebAssembly UI.
- `tests/Antro.Domain.Tests` contains domain unit tests.
- `tests/Antro.Application.Tests` contains application unit tests.
- `tests/Antro.Content.Tests` contains content unit tests.
- `docs/IMPLEMENTATION_RULES.md` contains the architectural implementation constraints for the MVP.

## Verification

Run the standard verification commands from the repository root:

```powershell
dotnet restore Antro.sln
dotnet build Antro.sln
dotnet test Antro.sln
```
