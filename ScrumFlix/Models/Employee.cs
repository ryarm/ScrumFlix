/*
 * File: /ScrumFlix/Models/Employee.cs
 * Description: Represents a ScrumFlix theater employee with contact information and role assignment.
 */

namespace ScrumFlix.Models;

/// <summary>
/// A theater employee with personal information, role assignment, and employment dates.
/// </summary>
[Table("employees")]
public class Employee
{
    /// <summary>Gets or sets the unique employee identifier.</summary>
    [Key]
    [Column("employee_id")]
    public int EmployeeId { get; set; }

    /// <summary>Gets or sets the employee's first name.</summary>
    [Required]
    [MaxLength(50)]
    [Column("first_name")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Gets or sets the employee's optional middle name.</summary>
    [MaxLength(50)]
    [Column("middle_name")]
    [Display(Name = "Middle Name")]
    public string? MiddleName { get; set; }

    /// <summary>Gets or sets the employee's last name.</summary>
    [Required]
    [MaxLength(50)]
    [Column("last_name")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>Gets or sets the date the employee started.</summary>
    [Column("employee_start_date")]
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    public DateTime EmployeeStartDate { get; set; }

    /// <summary>Gets or sets the optional date the employee ended employment.</summary>
    [Column("employee_end_date")]
    [DataType(DataType.Date)]
    [Display(Name = "End Date")]
    public DateTime? EmployeeEndDate { get; set; }

    /// <summary>Gets or sets the employee's date of birth.</summary>
    [Column("employee_dob")]
    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime EmployeeDob { get; set; }

    /// <summary>Gets or sets the employee's phone number.</summary>
    [Required]
    [MaxLength(20)]
    [Column("employee_phone")]
    [Display(Name = "Phone")]
    public string EmployeePhone { get; set; } = string.Empty;

    /// <summary>Gets or sets the employee's email address.</summary>
    [Required]
    [MaxLength(100)]
    [EmailAddress]
    [Column("employee_email")]
    [Display(Name = "Email")]
    public string EmployeeEmail { get; set; } = string.Empty;

    /// <summary>Gets or sets the employee's street address.</summary>
    [MaxLength(100)]
    [Column("employee_st_address")]
    [Display(Name = "Street Address")]
    public string? EmployeeStAddress { get; set; }

    /// <summary>Gets or sets the employee's city.</summary>
    [Required]
    [MaxLength(50)]
    [Column("employee_city")]
    [Display(Name = "City")]
    public string EmployeeCity { get; set; } = string.Empty;

    /// <summary>Gets or sets the employee's state abbreviation.</summary>
    [Required]
    [MaxLength(2)]
    [Column("employee_state")]
    [Display(Name = "State")]
    public string EmployeeState { get; set; } = string.Empty;

    /// <summary>Gets or sets the employee's ZIP code.</summary>
    [Required]
    [MaxLength(10)]
    [Column("employee_zipcode")]
    [Display(Name = "ZIP Code")]
    public string EmployeeZipcode { get; set; } = string.Empty;

    /// <summary>Gets or sets the role assigned to this employee.</summary>
    [Column("role_id")]
    public int RoleId { get; set; }

    // Navigation properties
    [ForeignKey(nameof(RoleId))]
    public EmployeeRole? Role { get; set; }
    public ICollection<ConcessionsSale> ConcessionsSales { get; set; } = new List<ConcessionsSale>();

    /// <summary>Returns the employee's full name.</summary>
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";
}
