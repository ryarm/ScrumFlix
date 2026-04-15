/*
 * File: /ScrumFlix/Models/Customer.cs
 * Description: Represents a registered customer who can purchase tickets and concessions.
 */

namespace ScrumFlix.Models;

/// <summary>
/// A registered ScrumFlix customer with contact information and category classification.
/// </summary>
[Table("customers")]
public class Customer
{
    /// <summary>Gets or sets the unique customer identifier.</summary>
    [Key]
    [Column("customer_id")]
    public int CustomerId { get; set; }

    /// <summary>Gets or sets the customer's first name.</summary>
    [Required]
    [MaxLength(50)]
    [Column("first_name")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Gets or sets the customer's last name.</summary>
    [Required]
    [MaxLength(50)]
    [Column("last_name")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>Gets or sets the customer's unique email address.</summary>
    [Required]
    [MaxLength(255)]
    [EmailAddress]
    [Column("customer_email")]
    [Display(Name = "Email")]
    public string CustomerEmail { get; set; } = string.Empty;

    /// <summary>Gets or sets the customer's phone number.</summary>
    [Required]
    [MaxLength(20)]
    [Column("customer_phone")]
    [Display(Name = "Phone")]
    public string CustomerPhone { get; set; } = string.Empty;

    /// <summary>Gets or sets the customer's date of birth.</summary>
    [Column("customer_dob")]
    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime? CustomerDob { get; set; }

    /// <summary>Gets or sets the pricing category for this customer.</summary>
    [MaxLength(20)]
    [Column("customer_category")]
    [Display(Name = "Category")]
    public string CustomerCategory { get; set; } = "Adult";

    // Navigation properties
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    /// <summary>Returns the customer's full name.</summary>
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";
}
