# Production stabilization review — November 7, 2026 (CT)

This update is based on the packaged source and an offline inspection of `Full_DB_20261106.sql`. No live Somee/Aiven access was available. The dump contains 2,121 `Logs` rows, spanning November 6–7, 2026; none are older than 30 days relative to November 7. Its `Logs` table has a primary key on `id` and no secondary index on `_ts`. The reported 98,072 rows are not the `Logs` row count in this dump; they may describe a different archive or another table. Treat the live database count as an open measurement.

| Concern | Source finding | Change | Runtime gate |
| --- | --- | --- | --- |
| EF row limit | The dump contains six log rows with `RowLimitingOperationWithoutOrderByWarning`. `HomeController.HomeDashboard` ordered showtime IDs and candidates, but the featured movie query had `Take(10)` without an order. | Order featured movies by title and unique MovieId before `Take(10)`. | Load home dashboard; no warning from that query; inspect featured order. |
| Export GET 405 | Both Admin forms already use `method="post"` and anti-forgery tokens. The dump shows two GET/405 log rows for each export action and no POST/405 export rows. | GET on either export action redirects authorized admins to the Exports form. POST remains required for downloads. | GET redirects to form without a download; form POST downloads a CSV/Excel file; non-admin stays blocked. |
| Data Protection | The dump contains 28 log rows mentioning ephemeral keys and 14 warning rows about missing XML encryption. No explicit durable key ring was configured. | Set `DataProtection__KeysPath` to an existing, writable, private, durable **Windows** directory outside the deployed app tree. This activates file persistence, stable application name, and Windows DPAPI machine encryption. Invalid configured paths fail startup. No host path is embedded in source. | Confirm keys appear in the configured directory; restart the app pool on the *same machine* and verify an anti-forgery token minted before restart remains valid. |
| MySQL Logs growth | This dump has 2,121 recent rows and zero currently eligible for 30-day deletion. `_ts` is a MySQL `timestamp` without an index. Backups exclude Logs by default. | Opt-in `LogRetention__Enabled=true` starts a daily worker after five minutes. It removes rows older than 30 days in at most 20 statements of 500 rows per sweep (10,000 per day). `AuditLog` is never touched. Leave it disabled for this dataset. | Measure the live table and retain required history before enabling. At a much larger row count, assess a secondary index on `(_ts, id)` in staging before running repeated bounded deletes. |
| Log routing | `LoggingConfiguration` loaded settings then overwrote minimum levels and always applied a second production noise filter, even in Development. EF created a separate logger factory in each DbContext configuration. | Use configured levels/filter and the host DI Serilog provider for EF. The MySQL sink still accepts Information+; email alert filtering remains separate. | In Development, confirm EF command Information logs appear per config; in Production, confirm expected application and EF warnings enter the intended sinks and the configured noise rule applies. |

## Deployment sequence

1. Keep the old deployment and database backup available. Provision a persistent folder outside the IIS site content root and web root, readable/writable only by the app identity. Set `DataProtection__KeysPath` in the host's environment configuration. Confirm it survives deployment and app pool recycle. The sample `C:\DZHosts\...\www...\keys` path inside the site tree is deliberately rejected.
2. Deploy the app and inspect startup logs. Confirm the configured key folder can be written, that key XML is protected with DPAPI, and that the app can decrypt a token after a same-machine recycle. DPAPI machine protection does not support a move to a different server without an additional migration plan (for example, certificate-backed shared storage).
3. The app's shopping cart and staff authentication state use `AddDistributedMemoryCache` sessions. Persisting Data Protection keys **does not preserve that session data** through an app recycle. Preserving active sessions requires a durable distributed session cache and a separate migration. Do not claim existing logins or carts will survive recycling based on keys alone.
4. Verify the exports from the Admin UI. An old GET bookmark should land on the parameter form, not trigger a download. A POST carries the anti-forgery token and must stay restricted to Admin.
5. The current dump has no logs old enough to delete. Leave `LogRetention__Enabled=false` until live counts and retention requirements justify cleanup. Capture any Logs history that must be kept. At large scale, consider adding `CREATE INDEX idx_logs_ts_id ON Logs (_ts, id)` in a staging copy and measure the impact before any production schema change. The first enabled sweep starts five minutes after process start; later sweeps occur approximately every 24 hours while the process remains alive. If the app frequently sleeps/recycles before five minutes, schedule a separate approved maintenance job. The worker has no database schema migration and assumes the sink's existing `Logs(id, _ts)` schema and DELETE permission.

## Verification commands

From the extracted project root on Windows with the private `ScrumFlix/sixlabors.lic` in place:

```powershell
dotnet build .\ScrumFlix\ScrumFlix.csproj -c Release
dotnet test .\tests\ScrumFlix.Tests\ScrumFlix.Tests.csproj -c Release
```

SQL is read-only until retention is explicitly enabled:

```sql
SELECT COUNT(*) AS total_rows FROM Logs;
SELECT COUNT(*) AS eligible_rows FROM Logs WHERE _ts < NOW() - INTERVAL 30 DAY;
SELECT MIN(_ts) AS oldest_row, MAX(_ts) AS newest_row FROM Logs;
```

The earlier Windows Debug run reported 92/92 unit tests and passed the Alpine CSP smoke test. This source update has not been compiled or run in this workspace because the .NET SDK and live services are unavailable. Release, browser, host recycling, export, and database gates remain open.
