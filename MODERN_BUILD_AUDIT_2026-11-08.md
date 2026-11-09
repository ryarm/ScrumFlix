# ScrumFlix modern build review — November 8, 2026 (CT)

## Source and decision

Base: `ScrumFlix_AlpineJS_Integration_Merged_2026-11-08.zip`, SHA-256 `02a049d4a26f2fb2af122516fe29534d251dcc7f313cbbdc44931ef1a0efd2c8`. This is an isolated candidate, not an update to the user's Windows checkout or deployed application. Preserve the private `sixlabors.lic` and `web.Production.config` locally; neither is included here. Do not publish before restoring and verifying the private transform or equivalent host secret configuration.

| Finding | Evidence | Action |
| --- | --- | --- |
| .NET 10 target | `ScrumFlix/ScrumFlix.csproj` targets `net10.0`; .NET 10 is supported through November 2028. | Retained. No speculative .NET 11 move. |
| EF Core/provider | Pomelo 9.0.0 supports EF Core 9.0.x and MySqlConnector 2.4.0+; no Pomelo 10 stable release found in the provider releases. Existing EF references were 9.0.17. | Updated EF Core, Design, and test Relational references to the EF 9 patch 9.0.22; retained Pomelo 9.0.0 and MySqlConnector 2.6.2. This candidate requires Windows restore/build/test. EF Core 9 support ends in November 2026; plan a separately qualified EF 10/provider migration when available. |
| Local EF CLI | `ScrumFlix/dotnet-tools.json` pinned `dotnet-ef` 10.0.6, with EF 9 runtime/design references. Root-level `dotnet tool restore` would not discover a manifest nested in `ScrumFlix/`. | Moved to `.config/dotnet-tools.json` at the solution root and pinned `dotnet-ef` 9.0.22, matching the EF major and patch. |
| Test runner | `global.json` selects Microsoft.Testing.Platform; solution includes the two xunit.v3 4.0.2 test projects. | Retained. Microsoft's .NET 10 MTP selection matches the previous Windows VSTest failure and subsequent pass. |
| Legacy project files | `ScrumFlix/ScrumFlix.Tests.csproj` and `ScrumFlix/ScrumFlix.IntegrationTests.csproj` still reference xUnit v2 but are not in `ScrumFlix.slnx`. | Left untouched to preserve user source; run the two `tests/` projects named by the solution. Review and remove or archive the legacy files only after confirming they are unused in local workflows. |
| Lock/reproducibility | No NuGet lockfiles or central package management; direct package versions are pinned. Frontend has a lockfile. No exact SDK version is pinned in `global.json`. | No unverified SDK pin or mass dependency rewrite. Consider NuGet lockfiles after a successful Windows restore, then validate locked-mode restore in CI. |
| Package notes | Several `.csproj` comments named versions older than the actual pins. | Corrected comments describing the already upgraded packages. |
| Production settings | Retention remains disabled by default; Data Protection key path remains empty by default. | No change to production opt-in behavior, data, or logging. |

## Validation performed here

- XML parsing of project/solution/props and JSON parsing of tool, global and frontend manifests: **PASS**.
- EF package-reference and local-tool version alignment; tool manifest placement: **PASS by source inspection**.
- Frontend `npm ci --offline --ignore-scripts --no-audit --no-fund`: **BLOCKED**, `ENOTCACHED` for the locked `xmlchars-2.2.0` tarball. No frontend source or lockfile was changed. The prior user Windows frontend smoke pass predates this audit, but the frontend bytes are unchanged.
- `dotnet restore`, Release build, .NET tests, dependency vulnerability/deprecation review, Windows x64 runtime, database, IIS/Somee deployment: **UNTESTED**. The SDK is not installed in this workspace. These source checks are not release acceptance.

## Windows qualification from extracted root

```powershell
dotnet --info
dotnet tool restore
dotnet tool run dotnet-ef -- --version
dotnet restore .\ScrumFlix.slnx
dotnet build .\ScrumFlix.slnx -c Release --no-restore
dotnet test .\tests\ScrumFlix.Tests\ScrumFlix.Tests.csproj -c Release --no-restore
dotnet test .\tests\ScrumFlix.IntegrationTests\ScrumFlix.IntegrationTests.csproj -c Release --no-restore
dotnet list .\ScrumFlix.slnx package --vulnerable --include-transitive
dotnet list .\ScrumFlix.slnx package --deprecated
Push-Location .\tests\frontend
npm ci
npm test
npm audit
Pop-Location
```

Inspect the first meaningful restore/build/test failure and repair that cause before deployment. The integration suite may require its documented test configuration. Run the browser and hosting checks in `SMOKE_TEST.md` and `PRODUCTION_STABILIZATION.md` separately. Avoid running migration or publish commands against production during this validation.

## Primary sources checked

- Microsoft .NET releases/support: https://learn.microsoft.com/en-us/dotnet/core/releases-and-support
- Microsoft EF tools guidance: https://learn.microsoft.com/en-us/ef/core/get-started/overview/install
- Microsoft local tool manifest discovery: https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install
- Microsoft `dotnet test` MTP selection: https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-test
- Pomelo 9 compatibility/release list: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/releases
- NuGet EF Core 9.0.22 and tool: https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/9.0.22 and https://www.nuget.org/packages/dotnet-ef/9.0.22
