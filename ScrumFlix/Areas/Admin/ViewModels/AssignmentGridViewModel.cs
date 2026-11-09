using Microsoft.AspNetCore.Mvc.Rendering;

namespace ScrumFlix.Areas.Admin.ViewModels;

public sealed class AssignmentGridViewModel
{
    public List<AssignmentRowViewModel> Rows { get; set; } = [];
    public int? LocationId { get; set; }
    public string? Search { get; set; }
    public string Sort { get; set; } = "start";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
    public int TotalPages => Math.Max(1, (TotalCount + PageSize - 1) / PageSize);
    public int FirstRow => TotalCount == 0 ? 0 : (Page - 1) * PageSize + 1;
    public int LastRow => Math.Min(TotalCount, Page * PageSize);
    public List<SelectListItem> Locations { get; set; } = [];
}
