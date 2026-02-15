# AGENTS Guide for `Necksaver_xr`

## Build
- Primary solution: `XRNeckSafer.sln` (repo root).
- Build command:
  - `dotnet build XRNeckSafer.sln`
- If `dotnet` is unavailable in your environment, use Visual Studio 2022 (or newer) and build the solution from the IDE.

## Test
- Test project: `XRNeckSaferApp.Tests/XRNeckSaferApp.Tests.csproj`.
- Run all tests:
  - `dotnet test XRNeckSafer.sln`
- Run only the RotationCalculator tests:
  - `dotnet test XRNeckSaferApp.Tests/XRNeckSaferApp.Tests.csproj`

## Coding conventions
- Language: C#.
- Use 4-space indentation and K&R brace style as used across the C# code in this repository.
- Prefer explicit, descriptive method/variable names (`PascalCase` for types/methods, `camelCase` for local variables/parameters).
- Keep formatting stable to produce clean diffs (one statement per line, avoid unnecessary compacting).
- Avoid unrelated refactors in focused PRs.

## RotationCalculator-specific expectations
- Preserve existing algorithm behavior when making formatting or safety updates.
- Add defensive checks for externally supplied collections/rows to avoid `NullReferenceException` and `IndexOutOfRangeException`.
- Prefer skipping malformed step rows rather than throwing.

## Dev environment notes
- This repo includes legacy .NET Framework and native projects; ensure required workloads are installed in Visual Studio for full solution builds.
- For CLI workflows, restore and build from the solution root.
