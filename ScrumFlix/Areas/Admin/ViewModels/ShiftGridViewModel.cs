namespace ScrumFlix.Areas.Admin.ViewModels;

public sealed class ShiftGridViewModel
{
    public List<ShiftRowViewModel> Rows { get; set; } = [];
    public int? LocationId { get; set; }
    public int? RoleId { get; set; }
    public DateTime? Date { get; set; }
    public string Sort { get; set; } = "start";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
    public int TotalCount { get; set; }
    public int TotalPages => Math.Max(1, (TotalCount + PageSize - 1) / PageSize);
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Locations { get; set; } = [];
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Roles { get; set; } = [];
}
