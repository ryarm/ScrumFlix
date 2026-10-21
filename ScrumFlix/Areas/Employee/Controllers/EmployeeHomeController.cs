/*
 * File:      Areas/Employee/Controllers/EmployeeHomeController.cs
 * Namespace: ScrumFlix.Areas.Employee.Controllers
 * Purpose:   Employee Portal home controller — landing page for RoleId 3 users.
 *
 *            ACTIONS:
 *              GET  /Employee/EmployeeHome/Index — Employee dashboard (home page)
 *
 *            ROUTING:
 *              Registered via the area route in Program.cs:
 *                endpoints.MapControllerRoute(
 *                    name:    "areaRoute",
 *                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
 *
 *            AUTHORIZATION:
 *              RoleGuard(3) on every action — RoleId <= 3 passes (Admin=1, Manager=2, Employee=3).
 *              No action may proceed without calling RoleGuard() as its first statement.
 *
 *            NO VIEWBAG FOR MODEL DATA:
 *              All view data travels through EmployeeDashboardViewModel.
 *              ViewData["Title"] is the only ViewData key permitted (layout title).
 *
 *            ASYNC THROUGHOUT:
 *              All DB reads use async EF Core calls (ToListAsync, FirstOrDefaultAsync).
 *              CreateExecutionStrategy is required if any future action adds an explicit
 *              transaction (standing rule — see StaffControllerBase comment header).
 *
 * Phase:   4 — Employee Area Scaffold
 * Author:  ScrumFlix Rebuild Team
 */

using ScrumFlix.Areas.Employee.ViewModels;

namespace ScrumFlix.Areas.Employee.Controllers;

/// <summary>
/// Handles the Employee Portal home / dashboard page.
/// Inherits <see cref="StaffControllerBase"/> for RoleGuard, LayoutViewModel population,
/// and session helpers (CurrentUserId, CurrentUserName, etc.).
/// </summary>
[Area("Employee")]
public sealed class EmployeeHomeController : StaffControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<EmployeeHomeController> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="EmployeeHomeController"/>.
    /// </summary>
    public EmployeeHomeController(AppDbContext db, ILogger<EmployeeHomeController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // ── Index ──────────────────────────────────────────────────────────────

    /// <summary>
    /// GET /Employee/EmployeeHome/Index
    /// Employee portal dashboard — primary landing page for RoleId 3 users.
    /// Displays the employee's profile summary, today's shifts, and upcoming
    /// shifts within the next 7 days.
    ///
    /// Phase 5 will extend this with real-time clock-in status from ITimeTrackingService.
    /// Phase 8 will add current pay period and recent paystub summary.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (RoleGuard(3) is { } redirect) return redirect;

        ViewData["Title"] = "My Dashboard";

        var userId      = CurrentUserId;
        var todayStart  = DateTime.Today;
        var todayEnd    = todayStart.AddDays(1);
        var weekEnd     = todayStart.AddDays(7);

        // ── Resolve Employee profile ─────────────────────────────────────
        // The FK lives on Users.EmployeeId, so we navigate User → Employee
        // rather than the reverse to avoid a subquery on the Employee table.
        var user = await _db.Users
            .AsNoTracking()
            .Include(u => u.Employee!)
                .ThenInclude(e => e.Location)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        var employee = user?.Employee;

        if (employee is null)
        {
            _logger.LogWarning(
                "EmployeeHomeController.Index: No Employee record found for UserId {UserId}. " +
                "Dashboard will render with session-only name and empty shift list.",
                userId);
        }

        // ── Fetch upcoming ScheduleAssignments (today + next 7 days) ────
        // ScheduleAssignments.UserId FK (not EmployeeId — per Phase 3 correction).
        // Shift.StartTime is a UTC DateTime; compare against DateTime.Today (local)
        // which is acceptable for demo/single-timezone deployments.
        // Phase 5 should thread Location.TimeZoneId through TimeZoneHelper for
        // accurate multi-timezone clock-in displays.
        var assignments = await _db.ScheduleAssignments
            .AsNoTracking()
            .Include(a => a.Shift!)
                .ThenInclude(s => s.Location)
            .Include(a => a.AssignmentArea)
            .Where(a => a.UserId == userId
                     && a.Shift!.StartTime >= todayStart
                     && a.Shift.StartTime <  weekEnd)
            .OrderBy(a => a.Shift!.StartTime)
            .Take(10)
            .ToListAsync();

        // ── Project to view model ────────────────────────────────────────
        var shiftVms = assignments.Select(a => new UpcomingShiftViewModel
        {
            StartTime       = a.Shift!.StartTime,
            EndTime         = a.Shift.EndTime,
            AssignmentArea  = a.AssignmentArea?.AreaName ?? "Unassigned",
            LocationName    = a.Shift.Location?.LocationName,
            IsToday         = a.Shift.StartTime >= todayStart && a.Shift.StartTime < todayEnd,
            FormattedDuration = a.Shift.FormattedDuration
        }).ToList();

        var vm = new EmployeeDashboardViewModel
        {
            FullName       = employee?.FullName ?? CurrentUserName ?? "Employee",
            LocationName   = employee?.Location?.LocationName,
            PayRate        = employee?.PayRate,
            HasEmployee    = employee is not null,
            TodayShifts    = shiftVms.Where(s => s.IsToday).ToArray(),
            UpcomingShifts = shiftVms
        };

        return View(vm);
    }
}
