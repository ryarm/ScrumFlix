/*
 * File:      Areas/Employee/ViewModels/EmployeeDashboardViewModel.cs
 * Namespace: ScrumFlix.Areas.Employee.ViewModels
 * Purpose:   View model for the Employee home dashboard (Phase 4 scaffold).
 *
 *            Populated by EmployeeHomeController.Index() — no ViewBag, no
 *            session reads in the view. All data flows through this model.
 *
 *            Phase 5 additions (TimeTracking): add ClockStatus, TodayHours,
 *            IsCurrentlyClockedIn, LastClockInTime once ITimeTrackingService exists.
 *
 *            Phase 8 additions (Paystubs): add RecentPaystubs list and current
 *            PayPeriod summary once IPaystubCalculationService exists.
 *
 * Phase:   4 — Employee Area Scaffold
 * Author:  ScrumFlix Rebuild Team
 */

namespace ScrumFlix.Areas.Employee.ViewModels;

/// <summary>
/// Top-level view model for the Employee Portal home dashboard.
/// </summary>
public sealed class EmployeeDashboardViewModel
{
    /// <summary>Employee's display name (from Employee.FullName, or session username fallback).</summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// Primary theater location name (Location.LocationName), or null if unassigned.
    /// </summary>
    public string? LocationName { get; init; }

    /// <summary>Hourly pay rate, or null if not set on the Employee record.</summary>
    public decimal? PayRate { get; init; }

    /// <summary>
    /// True if an Employee record is linked to this user account.
    /// False for accounts that have RoleId=3 but no Employee row (e.g. test accounts).
    /// The view uses this to show a profile-incomplete notice.
    /// </summary>
    public bool HasEmployee { get; init; }

    /// <summary>Shifts assigned to this user that start today (local time).</summary>
    public IReadOnlyList<UpcomingShiftViewModel> TodayShifts { get; init; }
        = Array.Empty<UpcomingShiftViewModel>();

    /// <summary>
    /// All shifts in the next 7 days, ordered by StartTime ascending.
    /// Includes today's shifts — the view can filter by <see cref="UpcomingShiftViewModel.IsToday"/>
    /// to avoid duplicating today's entries.
    /// </summary>
    public IReadOnlyList<UpcomingShiftViewModel> UpcomingShifts { get; init; }
        = Array.Empty<UpcomingShiftViewModel>();

    // ── Phase 5 stubs — uncomment when ITimeTrackingService is added ──────
    // public bool   IsCurrentlyClockedIn { get; init; }
    // public string ClockStatus          { get; init; } = "Not clocked in";
    // public decimal TodayHoursWorked    { get; init; }

    // ── Phase 8 stubs — uncomment when PaystubCalculationService is added ─
    // public string?  CurrentPayPeriodLabel  { get; init; }
    // public decimal? CurrentPeriodGrossPay  { get; init; }
    // public IReadOnlyList<PaystubSummaryViewModel> RecentPaystubs { get; init; }
    //     = Array.Empty<PaystubSummaryViewModel>();
}

/// <summary>
/// Summary of a single ScheduleAssignment for dashboard display.
/// </summary>
public sealed class UpcomingShiftViewModel
{
    /// <summary>Shift start (UTC from DB; format in the view using local timezone if needed).</summary>
    public DateTime StartTime { get; init; }

    /// <summary>Shift end.</summary>
    public DateTime EndTime { get; init; }

    /// <summary>AssignmentArea.AreaName (e.g. "Box Office", "Concessions").</summary>
    public string AssignmentArea { get; init; } = string.Empty;

    /// <summary>Location.LocationName for the shift, or null if the shift has no location.</summary>
    public string? LocationName { get; init; }

    /// <summary>True when the shift starts and ends within today's calendar day (local time).</summary>
    public bool IsToday { get; init; }

    /// <summary>Pre-computed duration string from Shift.FormattedDuration (e.g. "4h 0m").</summary>
    public string FormattedDuration { get; init; } = string.Empty;
}
