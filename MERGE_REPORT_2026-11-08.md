# ScrumFlix current-source merge — November 8, 2026 (CT)

## Source identity and scope

- User source: `ScrumFlix_AlpineJS_Integration_2026-11-07.zip`, SHA-256 `a755e99952cbe6c85436cced9377dc2fde6cb0a17e80ede164b341db77a116b1`.
- Prior stabilization source: the November 7 package revision with `PRODUCTION_STABILIZATION.md` and `LogRetentionService.cs`, SHA-256 `72f437bae1c7f89edafce2082ee64dec54a3825bca730470e266d25e6269a383`.
- The user source is authoritative for all overlapping files except the six targeted stabilization source/configuration changes listed below. The older package did not replace upgraded project files or the consumer layout.

## Preserved from the user source

- `ScrumFlix/ScrumFlix.csproj`: newer MailKit/MimeKit, MySqlConnector, HTTP resilience, MiniExcel, QuestPDF, QRCoder, Six Labors, PKCS, and TMDbLib pins.
- Both `tests/*/*.csproj`: xUnit v3 4.0.2 and runner 4.0.1, Test SDK 18.11.0, and the newer integration test package pin.
- `ScrumFlix/Views/Shared/_Layout.cshtml`: active footer and TMDB attribution.
- Six image/SVG assets under `ScrumFlix/wwwroot/images` present only in the user source.
- Updated `ScrumFlix/Properties/PublishProfiles/FTPProfile.pubxml` and all other nonprivate user-source files not named in the next section.

## Repairs merged into the user source

| File | Purpose |
| --- | --- |
| `ScrumFlix/Controllers/HomeController.cs` | Stable title/ID ordering before featured `Take(10)`. |
| `ScrumFlix/Areas/Admin/Controllers/AdminManageController.cs` | GET redirects for stale export bookmarks; POST remains protected. |
| `ScrumFlix/Infrastructure/LoggingConfiguration.cs` | Respect environment Serilog levels and filter rather than overriding them in code. |
| `ScrumFlix/Program.cs` | Host DI logging for EF, opt-in durable Data Protection key path, opt-in retention registration. |
| `ScrumFlix/appsettings.json` | Empty key path and disabled retention defaults. |
| `ScrumFlix/Services/LogRetentionService.cs` | New bounded 30-day cleanup worker, disabled until enabled in host configuration. |
| `PRODUCTION_STABILIZATION.md` | Dump-backed analysis and deployment gates. |
| `ALPINE_INTEGRATION_RELEASE_NOTES.md` | History and current merge status. |

## Packaging boundaries

The merged ZIP has one versioned source root and a SHA-256 manifest for its files. It excludes `.git`, `.vs`, `bin`, `obj`, `node_modules`, `*.user`, `sixlabors.lic`, and `web.Production.config` from the uploaded snapshot. The last file contains populated production connection/API/mail configuration. The original November 7 upload and your local repository remain untouched; keep both private files locally when building/publishing. `web.Production.config` is required by the existing FTP Release publish transform. Do not publish this extracted package until your private transform or an equivalent host-side secret configuration is restored and verified.

The uploaded Git index reports `ScrumFlix/sixlabors.lic` as tracked even though `.gitignore` lists it. The production transform and `*.pubxml.user` are not tracked. In the local repository, `git rm --cached -- ScrumFlix/sixlabors.lic` stops tracking the license without deleting your working copy. Check `git status` before committing. If the repository or original ZIP was shared, treat any historical copy of the license as exposed and request a replacement from Six Labors; removing it from the latest commit does not erase Git history.

## Verification and open gates

- Both input ZIPs passed archive integrity checks. Source comparison found 13 shared file differences and six user-only app assets. The targeted repair files were merged without replacing user package pins or layout changes.
- Static source/configuration checks and packaged manifest replay were performed. No .NET SDK is installed in this workspace, so this merged revision has **not** been compiled or unit-tested here. The earlier Windows 92/92 result predates the user's package upgrades and this merge.
- Run `dotnet build .\ScrumFlix\ScrumFlix.csproj -c Release`, then `dotnet test .\tests\ScrumFlix.Tests\ScrumFlix.Tests.csproj -c Release` from the extracted root on Windows. Run the frontend smoke and the browser/staging checks in `SMOKE_TEST.md` and `PRODUCTION_STABILIZATION.md` before deployment.
- The Data Protection key path must be provisioned on the host. Retention remains disabled; the supplied SQL dump had 2,121 recent Logs rows and no rows older than 30 days. No production schema or data were changed.
