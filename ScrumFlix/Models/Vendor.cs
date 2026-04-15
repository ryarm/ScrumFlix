/*
 * File: /ScrumFlix/Models/Vendor.cs
 * Description: Represents a concessions supplier/vendor that provides inventory items to theater locations.
 */

namespace ScrumFlix.Models;

/// <summary>
/// A vendor that supplies concession inventory items to ScrumFlix theater locations.
/// </summary>
[Table("vendors")]
public class Vendor
{
    /// <summary>Gets or sets the unique vendor identifier (UUID stored as binary).</summary>
    [Key]
    [Column("vendor_id")]
    public byte[] VendorId { get; set; } = Guid.NewGuid().ToByteArray();

    /// <summary>Gets or sets the vendor's company name.</summary>
    [Required]
    [MaxLength(100)]
    [Column("vendor_name")]
    [Display(Name = "Vendor Name")]
    public string VendorName { get; set; } = string.Empty;

    /// <summary>Gets or sets the vendor's street address.</summary>
    [MaxLength(255)]
    [Column("vendor_address")]
    [Display(Name = "Address")]
    public string? VendorAddress { get; set; }

    /// <summary>Gets or sets the vendor's city.</summary>
    [MaxLength(100)]
    [Column("vendor_city")]
    [Display(Name = "City")]
    public string? VendorCity { get; set; }

    /// <summary>Gets or sets the vendor's state abbreviation.</summary>
    [MaxLength(2)]
    [Column("vendor_state")]
    [Display(Name = "State")]
    public string? VendorState { get; set; }

    /// <summary>Gets or sets the vendor's ZIP code.</summary>
    [Column("vendor_zipcode")]
    [Display(Name = "ZIP")]
    public int? VendorZipcode { get; set; }

    /// <summary>Gets or sets the vendor's phone number.</summary>
    [Column("vendor_phone")]
    [Display(Name = "Phone")]
    //public long? VendorPhone { get; set; }
    public string? VendorPhone { get; set; }

    /// <summary>Gets or sets the primary contact name at this vendor.</summary>
    [MaxLength(100)]
    [Column("vendor_contact_name")]
    [Display(Name = "Contact Name")]
    public string? VendorContactName { get; set; }

    /// <summary>Gets or sets the vendor's contact email address.</summary>
    [MaxLength(255)]
    [EmailAddress]
    [Column("vendor_email")]
    [Display(Name = "Email")]
    public string? VendorEmail { get; set; }

    // Navigation properties
    public ICollection<Inventory> InventoryItems { get; set; } = new List<Inventory>();
    public ICollection<ConcessionsOrder> ConcessionsOrders { get; set; } = new List<ConcessionsOrder>();
}
