using Microsoft.AspNetCore.Mvc.Rendering;

namespace ScrumFlix.Areas.Admin.ViewModels;


// ── Index (full page) ───────────────────────────────────────────────────────

public class ScheduleIndexViewModel
{
    public string ActiveTab { get; set; } = "timeline";
    public int? SelectedLocationId { get; set; }
    public int SelectedYear { get; set; }
    public int SelectedMonth { get; set; }
    public ShiftGridViewModel ShiftGrid { get; set; } = new();
    public ShiftFormViewModel ShiftForm { get; set; } = new();
    public GanttViewModel Gantt { get; set; } = new();
    public List<SelectListItem> MonthList { get; set; } = [];
    public List<SelectListItem> LocationList { get; set; } = [];
    public AssignmentGridViewModel AssignmentGrid { get; set; } = new();
    public AssignmentFormViewModel AssignmentForm { get; set; } = new();
}
