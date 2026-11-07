# ScrumFlix Alpine integration — manual smoke test

**Build under test:** `ScrumFlix_AlpineJS_Integration_2026-09-26.zip`  
**Prepared:** September 26, 2026 (CDT)  
**Purpose:** Check the actual app in a browser after the selective Alpine migration. The included automated component test does not exercise database routes, Razor output, or the deployed Content Security Policy (CSP).

## The previously failing unit tests

The original Phase 4.3 ZIP and the first Alpine ZIP each had 68 passing and 10 failing unit tests. Eight failures were abbreviation expectations (`CST` versus `CS`, for example); two QR payload tests depended on those labels. The shared `TimeZoneHelper.BuildAbbreviation` now gives stable labels for ScrumFlix's seven supported U.S. regions, including Alaska `AKST/AKDT`, Hawaii `HST`, and Arizona `MST` year-round. The updated suite has **92 passing, 0 failing** tests: 78 existing cases plus 14 new cases for supported zones and IANA aliases. The browser checks below still matter for end-to-end behavior.

## Prepare a safe test session

1. Extract the new ZIP to a separate folder and open its `ScrumFlix.slnx` or project. Keep the earlier Phase 4.3 ZIP untouched.
2. Run the app against a **development or staging database**. Have access to a consumer session, Admin or Manager account, and Employee account. Use disposable movie, showtime, shift, and assignment records for deletion tests. Have at least one future showtime with available seats and one in-stock concession.
3. In the browser, open DevTools **Console** and **Network**. Enable **Preserve log** in Network. Test at desktop width and at a narrow/mobile width. Repeat the core checks in each theme you use (dark, light, red).
4. Note the application URL, browser/version, operating system, app build, test database, and tester/date in your results record. Do not enter real payment information or delete production records.

### Quick automated checks (PowerShell, from the extracted project root)

```powershell
dotnet build .\ScrumFlix\ScrumFlix.csproj
dotnet test .\tests\ScrumFlix.Tests\ScrumFlix.Tests.csproj
Push-Location .\tests\frontend
npm ci
npm test
Pop-Location
```

**Expected:** The build has zero errors; the frontend smoke test prints `Alpine CSP smoke passed`. The .NET unit suite should now report **92 passed, 0 failed**. If any test fails, record the exact name and investigate before release.

Run `dotnet test` from the extracted project root so the included `global.json` selects Microsoft Testing Platform for the xUnit v3 test projects. This requires a .NET 10 SDK. The former `Testing with VSTest target is no longer supported` error occurs before test execution and indicates the wrong runner was selected; if you still see it, confirm your working directory and that `global.json` was extracted beside `ScrumFlix.slnx`.

Keep your personal `sixlabors.lic` at `ScrumFlix/sixlabors.lic` in the extracted project. It is deliberately excluded from this ZIP and from Git. `Directory.Build.props` points the test projects to that local file without copying the license. A `No Six Labors license found` warning in a Debug test run means the file was not found from the test project's build; check the filename and location. The file must also cover each assembly that directly references a Six Labors package if it is an assembly-scoped key. Do not paste the license contents into test logs or issues.

## Browser checklist

Mark each row **Pass**, **Fail**, or **N/A** and record a screenshot, console error, or Network request for any failure. Run destructive cases only against disposable data.

| ID | Page and action | Expected result | Result |
|---|---|---|---|
| 01 | On Login, open Network and reload. Look for `/js/alpine/scrumflix-alpine.js` and `/js/alpine/alpine-csp.min.js`; inspect Console. Repeat on an Admin page. | Both files load once with HTTP 200 on each applicable layout. No `Alpine Expression Error`, `unsafe-eval`, blocked-script, or duplicate-initialization error. | ☐ Pass ☐ Fail ☐ N/A |
| 02 | Open `/Account/Login`. Toggle password with mouse and keyboard (Tab, Enter/Space). | Type switches password ↔ text. Eye icon, accessible label, and `aria-pressed` update. Username field and form submission still work. | ☐ Pass ☐ Fail ☐ N/A |
| 03 | Submit an intentionally invalid Login (test credentials only). | Existing validation banner and field error styling appear; toggle still works after the returned page. No blank banner on a clean GET. | ☐ Pass ☐ Fail ☐ N/A |
| 04 | On `/Account/ChangePassword`, toggle current, new, and confirm independently; submit mismatched new/confirm values in a test account. | Each field toggles independently with its own label; validation appears; hidden `IsForced` value and field labels remain correct. Do not change a real account password for this check. | ☐ Pass ☐ Fail ☐ N/A |
| 05 | Open a future showtime booking page with seats. Press `+` and `−`, including the min/max boundaries. | Quantity stays within the displayed bounds; input remains read-only; total equals price per ticket × quantity, formatted to two decimals; no reload. | ☐ Pass ☐ Fail ☐ N/A |
| 06 | Select multiple seats, then reduce ticket quantity below the selected-seat count. Add a ticket to cart on disposable data. | Excess selected seats are cleared, warning/seat count stay consistent, and posted quantity/seat data match the visible selection. No stale selection or second event handler. | ☐ Pass ☐ Fail ☐ N/A |
| 07 | If a sold-out showtime is available in staging, open it. | Sold-out controls stay disabled and the page has no Alpine exception or unintended `getQty()`/seat-picker error. | ☐ Pass ☐ Fail ☐ N/A |
| 08 | In `/Cart/CartReview`, use `+`, `−`, and direct number editing on separate disposable cart items. Inspect Network for each action. | One `UpdateQuantity` POST per action, correct `cartItemId` and quantity, anti-forgery accepted, and server-rendered line totals/cart totals update. Quantity remains in 0–20. | ☐ Pass ☐ Fail ☐ N/A |
| 09 | Reduce a disposable cart item from 1 to 0. | The item is removed through the existing server behavior; there is no duplicate POST or unrelated cart item change. | ☐ Pass ☐ Fail ☐ N/A |
| 10 | Click Clear Cart, then Cancel. Reopen it and press Escape. Finally, confirm on a disposable cart. | Opening and cancelling cause **no POST**; focus returns to Clear Cart; explicit Yes causes exactly one `ClearCart` POST and the cart empties. The confirmation never flashes visible during page load. | ☐ Pass ☐ Fail ☐ N/A |
| 11 | On consumer `/Concessions/ConcessionsCatalog`, adjust two different cards, directly enter `20` and an out-of-range value, then add one item. | Cards retain independent quantities; values clamp to 1–20; one AJAX request carries the displayed quantity and item ID; toast and cart badge update. Disabled out-of-stock controls remain disabled. | ☐ Pass ☐ Fail ☐ N/A |
| 12 | On Admin Movie Edit, Movie Catalog, and Admin Manage Showtimes, test a delete button using disposable eligible records. First Cancel/Escape, then confirm. | The correct item name/message appears, focus enters confirmation and returns on Cancel, no POST occurs until Yes, and confirming sends exactly one POST for the correct ID. Only one confirmation UI appears. | ☐ Pass ☐ Fail ☐ N/A |
| 13 | Temporarily disable JavaScript in DevTools and reload a page with a destructive Admin or Clear Cart button. Do **not** confirm anything. | The initial destructive control is a non-submitting button: clicking it cannot silently delete or clear data when Alpine fails to load. Re-enable JavaScript afterward. | ☐ Pass ☐ Fail ☐ N/A |
| 14 | On Admin Concessions Catalog, sort Item, Price, and Stock with mouse, then keyboard Enter/Space. Repeat a column to reverse direction. | Rows sort in both directions without a CSP error; table headers remain understandable and the controls have visible focus. | ☐ Pass ☐ Fail ☐ N/A |
| 15 | On Admin Backup, use Select all and Clear all. **Do not generate a backup for this check.** | Checkboxes and the selected-table count/button state update correctly; no inline-event CSP error. | ☐ Pass ☐ Fail ☐ N/A |
| 16 | On Admin Schedule, click a shift row and an assignment row to load their edit forms. Then use delete on disposable records and accept/decline `hx-confirm`. Repeat after an HTMX grid refresh. | Row click loads the correct form in its fixed column. Delete click does **not** also load the edit form. Decline sends no delete request; accept updates the grid in place without a bare partial page. SignalR refresh remains functional if a second session is available. | ☐ Pass ☐ Fail ☐ N/A |
| 17 | Open the Employee dashboard with an Employee account. | Dashboard/HTMX behavior is unchanged; Alpine is not required or loaded in `_EmployeeLayout`. | ☐ Pass ☐ Fail ☐ N/A |
| 18 | Repeat checks 02, 05, 10, and 12 in applicable themes and narrow viewport. Watch Console/Network. | Controls remain readable and usable; no hidden panel flashes, blocked scripts, console exceptions, or duplicate requests. | ☐ Pass ☐ Fail ☐ N/A |

## Timezone/QR regression check

1. Run the .NET unit suite and confirm **92 passed, 0 failed**. The prior ten abbreviation/QR failures are now covered by the shared-helper correction; new tests cover winter/summer for all seven configured zones and common IANA aliases.
2. Where the app displays or exports a QR ticket or concession receipt for a **summer** Central-time event, verify the suffix is `CDT`; for a **winter** event, verify `CST`. Use seeded historical/test data if needed rather than creating a past public showtime. Check email and ticket/concession exports if those workflows are available in staging.
3. For staging locations in Alaska, Hawaii, and Arizona, verify `AKST/AKDT`, `HST`, and year-round `MST`, respectively, at representative winter and summer dates. An `AST`, `ADT`, `CS`, or `CD` label is a regression.

## Pass criteria and reporting

The Alpine migration is ready for your environment when checks 01–06, 08–16, and 18 pass, with 07/17 marked N/A only if the corresponding data/account is unavailable, and with the full unit suite at 92/92 and the timezone/QR regression check passing.

For any failure, capture **test ID, page URL, exact action, expected/actual result, Console error, HTTP method/status/response for the relevant Network request, theme, browser, and a screenshot**. Avoid sharing passwords, anti-forgery tokens, customer information, or database credentials in logs/screenshots.
