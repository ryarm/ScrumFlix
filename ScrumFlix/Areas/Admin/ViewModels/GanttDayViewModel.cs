namespace ScrumFlix.Areas.Admin.ViewModels;

// ── Gantt Day ───────────────────────────────────────────────────────────────

public class GanttDayViewModel
{
    public DateTime Date { get; set; }
    public int RowHeightPx { get; set; } = 36;
    public List<GanttBarViewModel> Bars { get; set; } = [];
}
