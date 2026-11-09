using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using ScrumFlix.Hubs;


namespace ScrumFlix.Areas.Admin.Controllers;

/// <summary>
/// Handles all schedule management: shifts, the visual Gantt panel,
/// and employee schedule assignments.
///
/// HTMX pattern used throughout:
///   - GET  Index          → full page (first load)
///   - All other actions   → return PartialView so HTMX can swap
///                           just the affected section
///
/// SignalR pattern:
///   - After every mutating action, broadcast to the affected
///     location group so other connected clients auto-refresh.
///   - Client JS calls htmx.ajax() on receiving the event.
///
/// Role guard: Manager or above (RoleId <= 2). Applied per-action via
/// RoleGuard(2) from StaffControllerBase. [Authorize] is not used —
/// ScrumFlix uses session-based auth, not ASP.NET Core Identity.
/// </summary>
[Area("Admin")]
public class ScheduleController : StaffControllerBase
{
    private readonly AppDbContext _db;
    private readonly IHubContext<ScheduleHub> _hub;
    private readonly ILogger<ScheduleController> _logger;

    public ScheduleController(
        AppDbContext db,
        IHubContext<ScheduleHub> hub,
        ILogger<ScheduleController> logger)
    {
        _db = db;
        _hub = hub;
        _logger = logger;
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Writes a record to AuditLog. Call after db.SaveChanges() so
    /// ObjectId is populated, then call SaveChanges() again.
    /// </summary>
    private void Audit(
        string actionType,
        string tableName,
        int? objectId,
        string description,
        string? oldValues = null,
        string? newValues = null)
    {
        _db.AuditLogs.Add(new AuditLog
        {
            UserId = CurrentUserId ?? 0,
            ActionType = actionType,
            TableName = tableName,
            ObjectId = objectId,
            ActionTime = DateTime.Now,
            Description = description,
            OldValues = oldValues,
            NewValues = newValues
        });
    }

    /// <summary>
    /// Broadcasts a SignalR event to all clients watching the given location
    /// and fires-and-forgets (awaited in async actions, ignored on failure).
    /// </summary>
    private async Task BroadcastShiftsUpdated(int locationId) =>
        await _hub.Clients
            .Group(ScheduleHub.LocationGroup(locationId))
            .SendAsync("ShiftsUpdated", locationId);

    private async Task BroadcastAssignmentsUpdated(int locationId) =>
        await _hub.Clients
            .Group(ScheduleHub.LocationGroup(locationId))
            .SendAsync("AssignmentsUpdated", locationId);

    // ── Combo / select-list builders ────────────────────────────────────────

    private List<SelectListItem> GetRoleSelectList(int? selectedId = null) =>
        _db.Roles
            .OrderBy(r => r.RoleName)
            .Select(r => new SelectListItem
            {
                Value = r.RoleId.ToString(),
                Text = r.RoleName ?? "",
                Selected = r.RoleId == selectedId
            })
            .ToList();

    private List<SelectListItem> GetActiveLocationSelectList(int? selectedId = null) =>
        _db.Locations
            .Where(l => l.IsActive || l.LocationId == selectedId)
            .OrderBy(l => l.LocationName)
            .Select(l => new SelectListItem
            {
                Value = l.LocationId.ToString(),
                Text = (l.LocationName ?? "") + (l.IsActive ? "" : " (closed)"),
                Selected = l.LocationId == selectedId
            })
            .ToList();

    private List<SelectListItem> GetEmployeeSelectList(int? selectedUserId = null) =>
        _db.Users
            .Include(u => u.Employee)
            .ThenInclude(e => e!.Location)
            .Include(u => u.Role)
            .Where(u => u.IsActive && u.Employee != null && u.UserName != "web.sales")
            .OrderBy(u => u.Employee!.LastName)
            .ThenBy(u => u.Employee!.FirstName)
            .AsEnumerable()
            .Select(u => new SelectListItem
            {
                Value = u.UserId.ToString(),
                Text = (u.Employee!.FirstName ?? "") + " " + (u.Employee.LastName ?? "")
                           + " (" + (u.Role?.RoleName ?? "") + "; home: "
                           + (u.Employee.Location?.LocationName ?? "unassigned") + ")",
                Selected = u.UserId == selectedUserId
            })
            .ToList();

    /// <summary>
    /// Active assignment areas for the assignment form dropdown (Phase 3 —
    /// normalized lookup replacing the raw AssignmentName text input).
    /// </summary>
    private List<SelectListItem> GetAssignmentAreaSelectList(int? selectedId = null) =>
        _db.AssignmentAreas
            .Where(aa => aa.IsActive)
            .OrderBy(aa => aa.AreaName)
            .Select(aa => new SelectListItem
            {
                Value = aa.AssignmentAreaId.ToString(),
                Text = aa.AreaName,
                Selected = aa.AssignmentAreaId == selectedId
            })
            .ToList();

    /// <summary>
    /// Shift options for the assignment form (Phase 3 fix — the form previously
    /// reused the Employees list as a placeholder for this dropdown).
    /// </summary>
    private List<SelectListItem> GetShiftSelectList(int? selectedId = null) =>
        _db.Shifts
            .Include(sh => sh.Location)
            .Include(sh => sh.Role)
            .OrderBy(sh => sh.StartTime)
            .AsEnumerable()
            .Select(sh => new SelectListItem
            {
                Value = sh.ShiftId.ToString(),
                Text = (sh.Location!.LocationName ?? "") + " — "
                           + sh.StartTime.ToString("MM/dd h:mm tt") + " – "
                           + sh.EndTime.ToString("h:mm tt")
                           + " (" + (sh.Role != null ? (sh.Role.RoleName ?? "") : "") + ")",
                Selected = sh.ShiftId == selectedId,
                Group = new SelectListGroup { Name = sh.LocationId.ToString() }
            })
            .ToList();

    private List<SelectListItem> GetShowtimeSelectList(int? selectedId = null)
    {
        var items = _db.Showtimes
            .Include(s => s.Movie)
            .Where(s => s.IsActive)
            .OrderBy(s => s.StartTime)
            .Select(s => new SelectListItem
            {
                Value = s.ShowtimeId.ToString(),
                Text = (s.Movie!.Title ?? "") + " — " + s.StartTime.ToString("MM/dd/yyyy hh:mm tt"),
                Selected = s.ShowtimeId == selectedId
            })
            .ToList();

        // "None" sentinel — matches WinForms ShowtimeComboItem { ShowtimeId = null }
        items.Insert(0, new SelectListItem
        {
            Value = "",
            Text = "None",
            Selected = selectedId == null
        });

        return items;
    }

    /// <summary>
    /// Dynamically builds the month select list from actual shift data,
    /// replacing the hardcoded "May 2026" in the original WinForms form.
    /// </summary>
    private List<SelectListItem> GetShiftMonthSelectList(int? selectedYear = null, int? selectedMonth = null)
    {
        var months = _db.Shifts
            .Select(s => new { s.StartTime.Year, s.StartTime.Month })
            .Distinct()
            .OrderByDescending(m => m.Year)
            .ThenByDescending(m => m.Month)
            .ToList();

        return months
            .Select(m => new SelectListItem
            {
                Value = $"{m.Year}-{m.Month:D2}",
                Text = new DateTime(m.Year, m.Month, 1).ToString("MMMM yyyy"),
                Selected = m.Year == selectedYear && m.Month == selectedMonth
            })
            .ToList();
    }

    // ── Full page ────────────────────────────────────────────────────────────

    /// <summary>
    /// GET /Schedule
    /// Full-page load. Subsequent interactions use partials via HTMX.
    /// </summary>
    [HttpGet]
    public IActionResult Index(int? locationId, int? year, int? month, DateTime? weekStart, string? tab,
        int? filterLocationId, int? filterRoleId, DateTime? filterDate, string? filterSort, int filterPage = 1)
    {
        if (RoleGuard(2) is { } redirect) return redirect;
        // Default to first active location if none provided
        locationId ??= _db.Locations
            .Where(l => l.IsActive)
            .OrderBy(l => l.LocationName)
            .Select(l => (int?)l.LocationId)
            .FirstOrDefault();

        // Default to current month
        year ??= DateTime.Today.Year;
        month ??= DateTime.Today.Month;
        if (year < 1900 || year > 2100 || month < 1 || month > 12 ||
            (weekStart.HasValue && (weekStart.Value.Year < 1900 || weekStart.Value.Year > 2100)))
            return BadRequest();

        var activeTab = tab is "shifts" or "assignments" ? tab : "timeline";
        var vm = BuildIndexViewModel(locationId, year.Value, month.Value, weekStart, activeTab,
            filterLocationId ?? locationId, filterRoleId, filterDate, filterSort, filterPage);
        return View(vm);
    }

    // ── Shifts grid ─────────────────────────────────────────────────────────

    /// <summary>
    /// GET /Schedule/ShiftsGrid
    /// Returns the shifts table partial. Called by HTMX and by the
    /// client-side SignalR listener after a remote "ShiftsUpdated" event.
    /// </summary>
    [HttpGet]
    public IActionResult ShiftsGrid(int? filterLocationId, int? filterRoleId,
        DateTime? filterDate, string? filterSort, int filterPage = 1)
    {
        if (RoleGuard(2) is { } redirect) return redirect;
        return PartialView("_ShiftsGrid", GetShiftGrid(
            filterLocationId, filterRoleId, filterDate, filterSort, filterPage));
    }

    /// <summary>
    /// GET /Schedule/GetShift/{id}
    /// Returns the shift edit form partial pre-populated with the selected
    /// shift's values. Fired by hx-get on a grid row click.
    /// Fixes the original WinForms gridShifts_CellClick pattern.
    /// </summary>
    [HttpGet]
    public IActionResult GetShift(int id)
    {
        if (RoleGuard(2) is { } redirect) return redirect;
        var shift = _db.Shifts.FirstOrDefault(s => s.ShiftId == id);
        if (shift == null) return NotFound();

        var vm = new ShiftFormViewModel
        {
            ShiftId = shift.ShiftId,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            Roles = GetRoleSelectList(shift.RoleId),
            Locations = GetActiveLocationSelectList(shift.LocationId)
        };

        return PartialView("_ShiftForm", vm);
    }

    /// <summary>
    /// POST /Schedule/AddShift
    /// Creates a new shift. Returns the refreshed shifts grid partial
    /// (and triggers a SignalR broadcast to other connected clients).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddShift(ShiftFormViewModel form, int? filterLocationId, int? filterRoleId,
        DateTime? filterDate, string? filterSort, int filterPage = 1)
    {
        if (RoleGuard(2) is { } redirect) return redirect;
        if (!ModelState.IsValid)
            return ShiftFormError(form);

        if (form.EndTime <= form.StartTime)
        {
            ModelState.AddModelError(nameof(form.EndTime), "End time must be after start time.");
            return ShiftFormError(form);
        }

        if (!await _db.Locations.AnyAsync(l => l.LocationId == form.LocationId && l.IsActive))
        {
            ModelState.AddModelError(nameof(form.LocationId), "Select an open location.");
            return ShiftFormError(form);
        }

        var shift = new Shift
        {
            StartTime = form.StartTime,
            EndTime = form.EndTime,
            RoleId = form.RoleId,
            LocationId = form.LocationId
        };

        _db.Shifts.Add(shift);
        await _db.SaveChangesAsync();

        Audit("ADD_SHIFT", "Shifts", shift.ShiftId,
            "Added shift",
            newValues: $"StartTime={shift.StartTime}, EndTime={shift.EndTime}, RoleId={shift.RoleId}, LocationId={shift.LocationId}");

        await _db.SaveChangesAsync();

        _logger.LogInformation("Shift {ShiftId} added by User {UserId}", shift.ShiftId, CurrentUserId);

        await BroadcastShiftsUpdated(shift.LocationId);

        return PartialView("_ShiftsGrid", GetShiftGrid(filterLocationId, filterRoleId, filterDate, filterSort, filterPage));
    }

    /// <summary>
    /// POST /Schedule/UpdateShift
    /// Updates an existing shift. Returns the refreshed shifts grid partial.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateShift(ShiftFormViewModel form, int? filterLocationId, int? filterRoleId,
        DateTime? filterDate, string? filterSort, int filterPage = 1)
    {
        if (RoleGuard(2) is { } redirect) return redirect;
        if (!ModelState.IsValid)
            return ShiftFormError(form);

        if (form.EndTime <= form.StartTime)
        {
            ModelState.AddModelError(nameof(form.EndTime), "End time must be after start time.");
            return ShiftFormError(form);
        }

        if (!await _db.Locations.AnyAsync(l => l.LocationId == form.LocationId && l.IsActive))
        {
            ModelState.AddModelError(nameof(form.LocationId), "Select an open location.");
            return ShiftFormError(form);
        }

        var shift = await _db.Shifts.FirstOrDefaultAsync(s => s.ShiftId == form.ShiftId);
        if (shift == null) return NotFound();

        var oldValues = $"StartTime={shift.StartTime}, EndTime={shift.EndTime}, RoleId={shift.RoleId}, LocationId={shift.LocationId}";
        var oldLocationId = shift.LocationId;

        shift.StartTime = form.StartTime;
        shift.EndTime = form.EndTime;
        shift.RoleId = form.RoleId;
        shift.LocationId = form.LocationId;

        Audit("UPDATE_SHIFT", "Shifts", shift.ShiftId,
            "Updated shift",
            oldValues: oldValues,
            newValues: $"StartTime={shift.StartTime}, EndTime={shift.EndTime}, RoleId={shift.RoleId}, LocationId={shift.LocationId}");

        await _db.SaveChangesAsync();

        _logger.LogInformation("Shift {ShiftId} updated by User {UserId}", shift.ShiftId, CurrentUserId);

        // Broadcast to both old and new location in case location changed
        await BroadcastShiftsUpdated(shift.LocationId);
        if (oldLocationId != shift.LocationId)
            await BroadcastShiftsUpdated(oldLocationId);

        return PartialView("_ShiftsGrid", GetShiftGrid(filterLocationId, filterRoleId, filterDate, filterSort, filterPage));
    }

    /// <summary>
    /// POST /Schedule/DeleteShift
    /// Deletes a shift. Guards against deletion if assignments exist —
    /// mirrors the original WinForms check. Returns the refreshed grid.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteShift(int shiftId, int? filterLocationId, int? filterRoleId,
        DateTime? filterDate, string? filterSort, int filterPage = 1)
    {
        if (RoleGuard(2) is { } redirect) return redirect;
        var shift = await _db.Shifts.FirstOrDefaultAsync(s => s.ShiftId == shiftId);
        if (shift == null) return NotFound();

        bool hasAssignments = await _db.ScheduleAssignments
            .AnyAsync(a => a.ShiftId == shiftId);

        if (hasAssignments)
        {
            ModelState.AddModelError("",
                "This shift has schedule assignments. Delete those assignments before deleting the shift.");
            return PartialView("_ShiftsGrid", GetShiftGrid(filterLocationId, filterRoleId, filterDate, filterSort, filterPage));
        }

        Audit("DELETE_SHIFT", "Shifts", shift.ShiftId,
            "Deleted shift",
            oldValues: $"StartTime={shift.StartTime}, EndTime={shift.EndTime}, RoleId={shift.RoleId}, LocationId={shift.LocationId}");

        var locationId = shift.LocationId;

        _db.Shifts.Remove(shift);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Shift {ShiftId} deleted by User {UserId}", shiftId, CurrentUserId);

        await BroadcastShiftsUpdated(locationId);

        return PartialView("_ShiftsGrid", GetShiftGrid(filterLocationId, filterRoleId, filterDate, filterSort, filterPage));
    }

    // ── Visual schedule (Gantt) ─────────────────────────────────────────────

    /// <summary>
    /// GET /Schedule/ScheduleVisual?locationId=1&amp;year=2026&amp;month=5
    /// Returns the Gantt panel partial. Called by HTMX on location/month
    /// combo change (hx-trigger="change") and by the SignalR listener.
    ///
    /// Month list is now dynamic (from actual shift data) — fixes the
    /// hardcoded "May 2026" limitation in the original WinForms form.
    /// </summary>
    [HttpGet]
    public IActionResult ScheduleVisual(int locationId, string period, DateTime? weekStart)
    {
        if (RoleGuard(2) is { } redirect) return redirect;
        if (!DateTime.TryParseExact(period, "yyyy-MM",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var selectedMonth))
            return BadRequest();
        if (weekStart.HasValue && (weekStart.Value.Year < 1900 || weekStart.Value.Year > 2100))
            return BadRequest();
        var vm = BuildGanttViewModel(locationId, selectedMonth.Year, selectedMonth.Month, weekStart);
        return PartialView("_ScheduleVisual", vm);
    }

    // ── Assignments grid ────────────────────────────────────────────────────

    /// <summary>
    /// GET /Schedule/AssignmentsGrid?locationId=1
    /// Returns the assignments table partial.
    /// </summary>
    [HttpGet]
    public IActionResult AssignmentsGrid(int? locationId)
    {
        if (RoleGuard(2) is { } redirect) return redirect;
        var rows = GetAssignmentRows(locationId);
        return PartialView("_AssignmentsGrid", rows);
    }

    /// <summary>
    /// GET /Schedule/GetAssignment/{id}
    /// Returns the assignment edit form partial pre-populated.
    /// Fixes the unwired gridScheduleAssignments_CellClick bug in the original.
    /// </summary>
    [HttpGet]
    public IActionResult GetAssignment(int id)
    {
        if (RoleGuard(2) is { } redirect) return redirect;
        var assignment = _db.ScheduleAssignments
            .FirstOrDefault(a => a.AssignmentId == id);

        if (assignment == null) return NotFound();
        var workLocationId = _db.Shifts.Where(s => s.ShiftId == assignment.ShiftId)
            .Select(s => s.LocationId).FirstOrDefault();
        if (workLocationId == 0) return NotFound();

        var vm = new AssignmentFormViewModel
        {
            AssignmentId = assignment.AssignmentId,
            AssignmentAreaId = assignment.AssignmentAreaId,
            UserId = assignment.UserId,
            ShiftId = assignment.ShiftId,
            LocationId = workLocationId,
            ShowtimeId = assignment.ShowtimeId,
            Areas = GetAssignmentAreaSelectList(assignment.AssignmentAreaId),
            Employees = GetEmployeeSelectList(assignment.UserId),
            Locations = GetActiveLocationSelectList(workLocationId),
            Shifts = GetShiftSelectList(assignment.ShiftId),
            Showtimes = GetShowtimeSelectList(assignment.ShowtimeId)
        };

        return PartialView("_AssignmentForm", vm);
    }

    /// <summary>
    /// POST /Schedule/AddAssignment
    /// Creates a new schedule assignment with role-match and overlap
    /// validation — exact business rules from the original WinForms form.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAssignment(AssignmentFormViewModel form)
    {
        if (RoleGuard(2) is { } redirect) return redirect;
        if (!ModelState.IsValid)
            return AssignmentFormError(form);

        var (user, shift, error) = await ValidateAssignment(
            form.UserId, form.ShiftId, form.LocationId,
            form.AssignmentAreaId, excludeAssignmentId: null);

        if (error != null)
        {
            ModelState.AddModelError("", error);
            return AssignmentFormError(form);
        }

        var assignment = new ScheduleAssignment
        {
            AssignmentAreaId = form.AssignmentAreaId,
            UserId = user!.UserId,
            ShiftId = shift!.ShiftId,
            ShowtimeId = form.ShowtimeId == 0 ? null : form.ShowtimeId
        };

        _db.ScheduleAssignments.Add(assignment);
        await _db.SaveChangesAsync();

        Audit("ADD_SCHEDULE_ASSIGNMENT", "ScheduleAssignments", assignment.AssignmentId,
            $"Added schedule assignment (AreaId {assignment.AssignmentAreaId})",
            newValues: $"AssignmentAreaId={assignment.AssignmentAreaId}, UserId={assignment.UserId}, ShiftId={assignment.ShiftId}, ShowtimeId={assignment.ShowtimeId}");

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Assignment {AssignmentId} added by User {UserId}",
            assignment.AssignmentId, CurrentUserId);

        await BroadcastAssignmentsUpdated(shift!.LocationId);

        return PartialView("_AssignmentsGrid", GetAssignmentRows(shift.LocationId));
    }

    /// <summary>
    /// POST /Schedule/UpdateAssignment
    /// Updates an existing assignment with role-match and overlap validation.
    /// Overlap check excludes the current assignment (same fix as original).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateAssignment(AssignmentFormViewModel form)
    {
        if (RoleGuard(2) is { } redirect) return redirect;
        if (!ModelState.IsValid)
            return AssignmentFormError(form);

        var assignment = await _db.ScheduleAssignments
            .FirstOrDefaultAsync(a => a.AssignmentId == form.AssignmentId);

        if (assignment == null) return NotFound();

        var (user, shift, error) = await ValidateAssignment(
            form.UserId, form.ShiftId, form.LocationId,
            form.AssignmentAreaId, excludeAssignmentId: assignment.AssignmentId);

        if (error != null)
        {
            ModelState.AddModelError("", error);
            return AssignmentFormError(form);
        }

        var oldValues = $"AssignmentAreaId={assignment.AssignmentAreaId}, UserId={assignment.UserId}, ShiftId={assignment.ShiftId}, ShowtimeId={assignment.ShowtimeId}";
        var oldLocationId = (await _db.Shifts.FindAsync(assignment.ShiftId))?.LocationId ?? 0;

        assignment.AssignmentAreaId = form.AssignmentAreaId;
        assignment.UserId = user!.UserId;
        assignment.ShiftId = shift!.ShiftId;
        assignment.ShowtimeId = form.ShowtimeId == 0 ? null : form.ShowtimeId;

        Audit("UPDATE_SCHEDULE_ASSIGNMENT", "ScheduleAssignments", assignment.AssignmentId,
            $"Updated schedule assignment (AreaId {assignment.AssignmentAreaId})",
            oldValues: oldValues,
            newValues: $"AssignmentAreaId={assignment.AssignmentAreaId}, UserId={assignment.UserId}, ShiftId={assignment.ShiftId}, ShowtimeId={assignment.ShowtimeId}");

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Assignment {AssignmentId} updated by User {UserId}",
            assignment.AssignmentId, CurrentUserId);

        await BroadcastAssignmentsUpdated(shift!.LocationId);
        if (oldLocationId != shift.LocationId)
            await BroadcastAssignmentsUpdated(oldLocationId);

        return PartialView("_AssignmentsGrid", GetAssignmentRows(shift.LocationId));
    }

    /// <summary>
    /// POST /Schedule/DeleteAssignment
    /// Deletes a schedule assignment. Returns the refreshed assignments grid.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAssignment(int assignmentId)
    {
        if (RoleGuard(2) is { } redirect) return redirect;
        var assignment = await _db.ScheduleAssignments
            .Include(a => a.Shift)
            .Include(a => a.AssignmentArea)
            .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

        if (assignment == null) return NotFound();

        var locationId = assignment.Shift?.LocationId ?? 0;

        Audit("DELETE_SCHEDULE_ASSIGNMENT", "ScheduleAssignments", assignment.AssignmentId,
            $"Deleted schedule assignment '{assignment.AssignmentArea?.AreaName}'",
            oldValues: $"AssignmentAreaId={assignment.AssignmentAreaId}, UserId={assignment.UserId}, ShiftId={assignment.ShiftId}, ShowtimeId={assignment.ShowtimeId}");

        _db.ScheduleAssignments.Remove(assignment);
        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "Assignment {AssignmentId} deleted by User {UserId}",
            assignmentId, CurrentUserId);

        await BroadcastAssignmentsUpdated(locationId);

        return PartialView("_AssignmentsGrid", GetAssignmentRows(locationId));
    }

    // ── Private query helpers ────────────────────────────────────────────────

    private ShiftGridViewModel GetShiftGrid(int? locationId, int? roleId,
        DateTime? date, string? sort, int page)
    {
        const int pageSize = 25;
        if (locationId is null or <= 0)
            locationId = _db.Locations.AsNoTracking()
                .Where(l => l.IsActive).OrderBy(l => l.LocationName)
                .Select(l => (int?)l.LocationId).FirstOrDefault();
        var query = _db.Shifts.AsNoTracking().AsQueryable();
        if (locationId > 0) query = query.Where(s => s.LocationId == locationId);
        if (roleId > 0) query = query.Where(s => s.RoleId == roleId);
        if (date.HasValue)
        {
            var start = date.Value.Date;
            if (start >= new DateTime(9999, 12, 31))
                query = query.Where(s => s.StartTime == start);
            else
            {
                var end = start.AddDays(1);
                query = query.Where(s => s.StartTime >= start && s.StartTime < end);
            }
        }

        var order = sort is "location" or "role" or "start_desc" ? sort : "start";
        var ordered = order switch
        {
            "location" => query.OrderBy(s => s.Location!.LocationName).ThenBy(s => s.StartTime),
            "role" => query.OrderBy(s => s.Role!.RoleName).ThenBy(s => s.StartTime),
            "start_desc" => query.OrderByDescending(s => s.StartTime),
            _ => query.OrderBy(s => s.StartTime)
        };
        var total = query.Count();
        var lastPage = Math.Max(1, (total + pageSize - 1) / pageSize);
        page = Math.Clamp(page, 1, lastPage);
        var rows = ordered.ThenBy(s => s.ShiftId)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(s => new ShiftRowViewModel
            {
                ShiftId = s.ShiftId,
                LocationName = s.Location!.LocationName ?? "",
                RoleName = s.Role!.RoleName ?? "",
                StartTime = s.StartTime,
                EndTime = s.EndTime
            }).ToList();

        return new ShiftGridViewModel
        {
            Rows = rows, TotalCount = total, Page = page, PageSize = pageSize,
            LocationId = locationId, RoleId = roleId, Date = date, Sort = order,
            Locations = GetActiveLocationSelectList(locationId),
            Roles = GetRoleSelectList(roleId)
        };
    }

    private List<AssignmentRowViewModel> GetAssignmentRows(int? locationId) =>
        _db.ScheduleAssignments
            .Include(a => a.AssignmentArea)
            .Include(a => a.User).ThenInclude(u => u!.Employee)
            .Include(a => a.Shift).ThenInclude(s => s!.Role)
            .Include(a => a.Shift).ThenInclude(s => s!.Location)
            .Include(a => a.Showtime).ThenInclude(s => s!.Movie)
            .Where(a => locationId == null || a.Shift!.LocationId == locationId)
            .OrderBy(a => a.Shift!.StartTime)
            .Select(a => new AssignmentRowViewModel
            {
                AssignmentId = a.AssignmentId,
                AreaName = a.AssignmentArea!.AreaName ?? "",
                EmployeeName = a.User!.Employee!.FullName ?? "",
                RoleName = a.Shift!.Role!.RoleName ?? "",
                LocationName = a.Shift.Location!.LocationName ?? "",
                StartTime = a.Shift.StartTime,
                EndTime = a.Shift.EndTime,
                ShowtimeTitle = a.Showtime != null ? (a.Showtime.Movie!.Title ?? "None") : "None"
            })
            .ToList();

    private GanttViewModel BuildGanttViewModel(int locationId, int year, int month, DateTime? weekStart = null)
    {
        var startMonth = new DateTime(year, month, 1);
        var requestedDate = weekStart ?? (year == DateTime.Today.Year && month == DateTime.Today.Month
            ? DateTime.Today : startMonth);
        var firstDay = requestedDate.Date.AddDays(-(((int)requestedDate.DayOfWeek + 6) % 7));
        var endWeek = firstDay.AddDays(7);

        // The late shift ends after midnight; include its complete bar and the
        // earliest 7:30 a.m. shift in the same theater-day timeline.
        const int startHour = 7;
        const int endHour = 26;
        const int hourWidthPx = 70;
        const int timelineLeftPx = 170;

        var shifts = _db.Shifts.AsNoTracking()
            .Include(s => s.Role)
            .Where(s => s.LocationId == locationId &&
                        s.StartTime >= firstDay && s.StartTime < endWeek)
            .OrderBy(s => s.StartTime)
            .ThenBy(s => s.ShiftId)
            .ToList();

        var shiftIds = shifts.Select(s => s.ShiftId).ToArray();
        var assignedNames = shiftIds.Length == 0
            ? new Dictionary<int, string>()
            : _db.ScheduleAssignments.AsNoTracking()
                .Where(a => shiftIds.Contains(a.ShiftId))
                .Select(a => new
                {
                    a.ShiftId,
                    Name = a.User!.Employee!.FirstName + " " + a.User.Employee.LastName
                })
                .ToList()
                .GroupBy(a => a.ShiftId)
                .ToDictionary(g => g.Key,
                    g => string.Join(", ", g.Select(a => a.Name).OrderBy(n => n)));

        var dayGroups = new List<GanttDayViewModel>();
        foreach (var date in Enumerable.Range(0, 7).Select(offset => firstDay.AddDays(offset)))
        {
            var day = new GanttDayViewModel { Date = date };
            var laneEnds = new List<DateTime>();
            foreach (var shift in shifts.Where(s => s.StartTime.Date == date)
                         .OrderBy(s => s.StartTime)
                         .ThenBy(s => s.EndTime).ThenBy(s => s.ShiftId))
            {
                // Allocate the first free lane. Overlapping shifts must never
                // share a vertical position or one role will conceal another.
                int lane = laneEnds.FindIndex(end => end <= shift.StartTime);
                if (lane < 0)
                {
                    lane = laneEnds.Count;
                    laneEnds.Add(shift.EndTime);
                }
                else
                {
                    laneEnds[lane] = shift.EndTime;
                }

                var names = assignedNames.GetValueOrDefault(shift.ShiftId, "Unfilled");
                day.Bars.Add(new GanttBarViewModel
                {
                    ShiftId = shift.ShiftId,
                    RoleName = shift.Role?.RoleName ?? "",
                    RoleColor = GetRoleColor(shift.Role?.RoleName),
                    AssignmentSummary = names,
                    Label = $"{shift.StartTime:h:mm tt} – {shift.EndTime:h:mm tt}",
                    LeftPx = timelineLeftPx + (int)((shift.StartTime -
                        shift.StartTime.Date.AddHours(startHour)).TotalHours * hourWidthPx),
                    WidthPx = Math.Max(60, (int)((shift.EndTime - shift.StartTime).TotalHours * hourWidthPx)),
                    TopPx = 4 + lane * 30
                });
            }
            day.RowHeightPx = Math.Max(36, 8 + laneEnds.Count * 30);
            dayGroups.Add(day);
        }

        return new GanttViewModel
        {
            LocationId = locationId,
            Year = year,
            Month = month,
            WeekStart = firstDay,
            StartHour = startHour,
            EndHour = endHour,
            HourWidthPx = hourWidthPx,
            TimelineLeftPx = timelineLeftPx,
            Days = dayGroups,
            IsEmpty = shifts.Count == 0
        };
    }

    private ScheduleIndexViewModel BuildIndexViewModel(int? locationId, int year, int month, DateTime? weekStart, string activeTab,
        int? filterLocationId, int? filterRoleId, DateTime? filterDate, string? filterSort, int filterPage)
    {
        return new ScheduleIndexViewModel
        {
            ActiveTab = activeTab,
            SelectedLocationId = locationId,
            SelectedYear = year,
            SelectedMonth = month,
            // Shifts section
            ShiftGrid = activeTab == "shifts"
                ? GetShiftGrid(filterLocationId, filterRoleId, filterDate, filterSort, filterPage)
                : new ShiftGridViewModel(),
            ShiftForm = activeTab == "shifts" ? new ShiftFormViewModel
            {
                Roles = GetRoleSelectList(),
                Locations = GetActiveLocationSelectList()
            } : new ShiftFormViewModel(),

            // Gantt section
            Gantt = activeTab == "timeline"
                ? BuildGanttViewModel(locationId ?? 0, year, month, weekStart) : new GanttViewModel(),
            MonthList = activeTab == "timeline" ? GetShiftMonthSelectList(year, month) : [],
            LocationList = activeTab is "timeline" or "assignments"
                ? GetActiveLocationSelectList(locationId) : [],

            // Assignments section
            Assignments = activeTab == "assignments" ? GetAssignmentRows(locationId) : [],
            AssignmentForm = activeTab == "assignments" ? new AssignmentFormViewModel
            {
                LocationId = locationId ?? 0,
                Locations = GetActiveLocationSelectList(locationId),
                Areas = GetAssignmentAreaSelectList(),
                Employees = GetEmployeeSelectList(),
                Shifts = GetShiftSelectList(),
                Showtimes = GetShowtimeSelectList()
            } : new AssignmentFormViewModel()
        };
    }

    /// <summary>
    /// Shared business-rule validation for both AddAssignment and UpdateAssignment.
    /// Returns the hydrated User + Shift on success, or an error string on failure.
    /// </summary>
    private async Task<(User? user, Shift? shift, string? error)> ValidateAssignment(
        int userId, int shiftId, int locationId, int assignmentAreaId, int? excludeAssignmentId)
    {
        // Rule 0 (Phase 3): assignment area must exist and be active
        bool areaOk = await _db.AssignmentAreas
            .AnyAsync(aa => aa.AssignmentAreaId == assignmentAreaId && aa.IsActive);

        if (!areaOk)
            return (null, null, "Select a valid assignment area.");

        var user = await _db.Users
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        var shift = await _db.Shifts
            .FirstOrDefaultAsync(s => s.ShiftId == shiftId);

        if (user == null || user.Employee == null)
            return (null, null, "Employee not found.");

        if (!user.IsActive)
            return (null, null, "This employee account is inactive.");

        if (shift == null)
            return (null, null, "Shift not found.");

        if (locationId <= 0 || shift.LocationId != locationId)
            return (null, null, "Select a shift at the chosen work location.");

        // Home location is advisory: employees can cover shifts at another
        // theater. The work location comes from the shift itself.
        bool locationIsOpen = await _db.Locations.AnyAsync(l =>
            l.LocationId == locationId && l.IsActive);
        if (!locationIsOpen)
        {
            bool unchangedHistoricalAssignment = excludeAssignmentId.HasValue &&
                await _db.ScheduleAssignments.AsNoTracking().AnyAsync(a =>
                    a.AssignmentId == excludeAssignmentId.Value &&
                    a.ShiftId == shiftId && a.UserId == userId);
            if (!unchangedHistoricalAssignment)
                return (null, null, "New assignments cannot be made at a closed location.");
        }

        // Rule 1: role must match (mirrors WinForms check exactly)
        if (user.RoleId != shift.RoleId)
            return (null, null, "This employee's role does not match the selected shift's role.");

        // Rule 2: no overlapping assignments for this user (excludes self on update)
        var overlapQuery = _db.ScheduleAssignments
            .Include(a => a.Shift)
            .Where(a =>
                a.UserId == userId &&
                shift.StartTime < a.Shift!.EndTime &&
                a.Shift.StartTime < shift.EndTime);

        if (excludeAssignmentId.HasValue)
            overlapQuery = overlapQuery.Where(a => a.AssignmentId != excludeAssignmentId.Value);

        bool hasOverlap = await overlapQuery.AnyAsync();

        if (hasOverlap)
            return (null, null, "This employee already has an overlapping schedule assignment.");

        return (user, shift, null);
    }

    // ── Form repopulation (on validation failure) ───────────────────────────

    private IActionResult ShiftFormError(ShiftFormViewModel form)
    {
        if (Request.Headers.ContainsKey("HX-Request"))
            Response.Headers["HX-Retarget"] = "#shift-form-container";
        return PartialView("_ShiftForm", RepopulateShiftForm(form));
    }

    private IActionResult AssignmentFormError(AssignmentFormViewModel form)
    {
        if (Request.Headers.ContainsKey("HX-Request"))
            Response.Headers["HX-Retarget"] = "#assignment-form-container";
        return PartialView("_AssignmentForm", RepopulateAssignmentForm(form));
    }

    private ShiftFormViewModel RepopulateShiftForm(ShiftFormViewModel form)
    {
        form.Roles = GetRoleSelectList(form.RoleId);
        form.Locations = GetActiveLocationSelectList(form.LocationId);
        return form;
    }

    private AssignmentFormViewModel RepopulateAssignmentForm(AssignmentFormViewModel form)
    {
        form.Areas = GetAssignmentAreaSelectList(form.AssignmentAreaId);
        form.Employees = GetEmployeeSelectList(form.UserId);
        form.Locations = GetActiveLocationSelectList(form.LocationId);
        form.Shifts = GetShiftSelectList(form.ShiftId);
        form.Showtimes = GetShowtimeSelectList(form.ShowtimeId);
        return form;
    }

    // ── Role color (mirrors WinForms GetRoleColor exactly) ──────────────────

    private static string GetRoleColor(string? roleName) => roleName switch
    {
        "Admin" => "#F08080", // LightCoral
        "Manager" => "#F0E68C", // Khaki
        "Employee" => "#87CEEB", // LightSkyBlue
        _ => "#D3D3D3"  // LightGray
    };
}
