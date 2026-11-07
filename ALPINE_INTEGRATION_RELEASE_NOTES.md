# ScrumFlix Alpine integration — 2026-09-26 CDT

Source baseline: `ScrumFlix_Phase4_3_ProgressFramework_2026-09-26(3).zip`. This package is a full source copy with the selective Alpine migration applied. It does not alter the original baseline archive.

## Implemented

- Self-hosted `@alpinejs/csp` **3.17.4** in both consumer and Admin layouts; kept the existing `script-src` policy with no `unsafe-eval` addition. The Employee layout does not load Alpine. The provider registry precedes the deferred Alpine core.
- Added `passwordVisibility`, `ticketQuantity`, `cartQuantity`, `concessionQuantity`, and `confirmation` providers. Retained ASP.NET Tag Helpers, field IDs, route parameters, anti-forgery tokens, and the booking input's `readonly` state.
- Preserved `window.getQty()` and the non-bubbling `qtychanged` event on `#qtyInput` for the unchanged `seatPicker.js`; kept quantity `0..20` for cart and `1..20` for concessions.
- Kept `login.js` and `account-change-password.js` validation behavior; kept concession fetch/toast logic. Removed obsolete `booking.js`, `cart-review.js`, and `sf-confirm.js` after moving all their consumers.
- Consolidated movie confirmations by removing the duplicate listener from `sf-movie-genre.js`. Destructive opener buttons are non-submitting; the explicit confirm action uses `requestSubmit()`. Added `x-cloak`, focus return, Escape cancellation, and accessible confirmation groups.
- Replaced the active inline click attributes in Schedule grids, Admin Backup, and Admin Concessions with external listeners. HTMX Schedule `hx-confirm` remains HTMX-owned. Updated the misleading HTMX client/server version comments without changing versions.
- Added a repeatable Alpine CSP component smoke test under `tests/frontend/`.
- Corrected shared time-zone abbreviations for the seven configured US regions, including Alaska, Hawaii, and Arizona. The existing ten abbreviation/QR failures are resolved; added 14 zone/alias test cases.
- Added a root `global.json` selecting `Microsoft.Testing.Platform` for `dotnet test` with the .NET 10 SDK and the solution's xUnit v3 test projects. This addresses the Windows error that says the VSTest target is no longer supported. Run test commands from this extracted project root.
- Added `Directory.Build.props` to locate a developer's private `ScrumFlix/sixlabors.lic` for test projects. The previous archive contained this license file; the current archive excludes it and `.gitignore` now ignores it. Keep your own valid license at that path after extracting the package. Do not commit or distribute an unrestricted license key.

## Verification performed

| Check | Result |
|---|---|
| `dotnet build ScrumFlix/ScrumFlix.csproj` with .NET SDK 10.0.401 | Passed; 0 warnings, 0 errors. Razor views compiled. |
| `npm ci` and `npm test` under `tests/frontend` | Passed Alpine CSP component smoke test in jsdom. |
| `node --check` on modified application scripts | Passed. |
| Unit suite on the user's Windows x64 machine after the MTP runner fix | 92 passed, 0 failed, 0 skipped. The Debug build emitted a `No Six Labors license found` warning from the test project; this package now points the test build to the user's existing file. That warning fix has not been re-run on Windows. |

The browser-only acceptance checks still need a local run with the deployed CSP: Login and ChangePassword invalid POST, booking seat trim, cart and concession POSTs, Admin delete confirmation/focus, and Schedule row edit versus delete after HTMX swaps. No live site or database was available in this workspace. The frontend smoke test exercises Alpine itself but does not replace those integrated browser checks.

On Windows, `dotnet build` and the frontend `npm test` passed in the user's extracted package. After adding `global.json`, `dotnet test` executed and all 92 tests passed. The new license path configuration still needs a Windows rerun to confirm it removes the Debug warning and permits Release builds. This workspace has no `dotnet` executable.

## Local commands

```bash
dotnet build ScrumFlix/ScrumFlix.csproj
dotnet test tests/ScrumFlix.Tests/ScrumFlix.Tests.csproj
cd tests/frontend && npm ci && npm test
```

## Vendored Alpine provenance

`ScrumFlix/wwwroot/js/alpine/alpine-csp.min.js` came from the official npm tarball `@alpinejs/csp@3.17.4`, path `package/dist/cdn.min.js`. SHA-256: `0d18d7f8d7910e2e0212f0f056b12f50bebc3abb7d88d2f7c7cb4c336fe4519a`. The package declares MIT licensing; the license text is included alongside the asset. To reproduce, extract that exact tarball and verify the hash before replacing the vendored file.
