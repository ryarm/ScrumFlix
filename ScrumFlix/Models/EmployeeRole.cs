/*
 * File: /ScrumFlix/Models/EmployeeRole.cs
 * Description: Represents a job role classification for ScrumFlix theater employees.
 */

namespace ScrumFlix.Models;

/// <summary>
/// A job role that can be assigned to theater employees (e.g., Manager, Usher, Concessions Associate).
/// </summary>
[Table("employee_roles")]
public class EmployeeRole
{
    /// <summary>Gets or sets the unique role identifier.</summary>
    [Key]
    [Column("role_id")]
    public int RoleId { get; set; }

    /// <summary>Gets or sets the name of this role.</summary>
    [Required]
    [MaxLength(255)]
    [Column("role_name")]
    [Display(Name = "Role Name")]
    public string RoleName { get; set; } = string.Empty;

    /// <summary>Gets or sets the description of responsibilities for this role.</summary>
    [Required]
    [MaxLength(255)]
    [Column("role_description")]
    [Display(Name = "Description")]
    public string RoleDescription { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
