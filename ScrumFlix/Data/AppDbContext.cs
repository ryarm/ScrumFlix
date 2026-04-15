/*
 * File: /ScrumFlix/Data/AppDbContext.cs
 * Description: Entity Framework Core database context for ScrumFlix, mapping all models to MySQL tables.
 */

using Microsoft.EntityFrameworkCore;

namespace ScrumFlix.Data;

/// <summary>
/// The primary EF Core database context for the ScrumFlix cinema application.
/// Configures all entity mappings and relationships for MySQL via Pomelo.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of AppDbContext with the provided options.
    /// </summary>
    /// <param name="options">DbContext configuration options (connection string, provider).</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ── DbSets ─────────────────────────────────────────────────────────────────

    /// <summary>Theater locations in the ScrumFlix chain.</summary>
    public DbSet<Location> Locations { get; set; }

    /// <summary>Screening rooms within each theater location.</summary>
    public DbSet<TheaterRoom> TheaterRooms { get; set; }

    /// <summary>Movies available in the system.</summary>
    public DbSet<Movie> Movies { get; set; }

    /// <summary>Scheduled movie showings.</summary>
    public DbSet<ScheduledShow> ScheduledShows { get; set; }

    /// <summary>Ticket price tier definitions.</summary>
    public DbSet<PriceTier> PriceTiers { get; set; }

    /// <summary>Purchased movie tickets.</summary>
    public DbSet<Ticket> Tickets { get; set; }

    /// <summary>Registered customers.</summary>
    public DbSet<Customer> Customers { get; set; }

    /// <summary>Employee role definitions.</summary>
    public DbSet<EmployeeRole> EmployeeRoles { get; set; }

    /// <summary>Theater employees.</summary>
    public DbSet<Employee> Employees { get; set; }

    /// <summary>Concession product suppliers.</summary>
    public DbSet<Vendor> Vendors { get; set; }

    /// <summary>Concession inventory items per location.</summary>
    public DbSet<Inventory> Inventory { get; set; }

    /// <summary>Location-specific concession pricing records.</summary>
    public DbSet<ConcessionsPricing> ConcessionsPricing { get; set; }

    /// <summary>Completed concessions sale transactions.</summary>
    public DbSet<ConcessionsSale> ConcessionsSales { get; set; }

    /// <summary>Line items within each concessions sale.</summary>
    public DbSet<ConcessionsSalesItem> ConcessionsSalesItems { get; set; }

    /// <summary>Vendor purchase orders for restocking inventory.</summary>
    public DbSet<ConcessionsOrder> ConcessionsOrders { get; set; }

    // ── Model Configuration ────────────────────────────────────────────────────

    /// <summary>
    /// Configures entity relationships, keys, and constraints using the Fluent API.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure entities.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Vendor: binary(16) primary key ──
        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.Property(v => v.VendorId).HasColumnType("binary(16)");
        });

        // ── Inventory: binary(16) primary key and FK to vendor ──
        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.Property(i => i.ItemId).HasColumnType("binary(16)");
            entity.Property(i => i.VendorId).HasColumnType("binary(16)");

            entity.HasOne(i => i.Vendor)
                  .WithMany(v => v.InventoryItems)
                  .HasForeignKey(i => i.VendorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(i => i.Location)
                  .WithMany(l => l.InventoryItems)
                  .HasForeignKey(i => i.LocationId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ── ConcessionsPricing: binary(16) FK to inventory ──
        modelBuilder.Entity<ConcessionsPricing>(entity =>
        {
            entity.Property(cp => cp.ItemId).HasColumnType("binary(16)");

            entity.HasOne(cp => cp.Inventory)
                  .WithMany(i => i.ConcessionsPricings)
                  .HasForeignKey(cp => cp.ItemId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(cp => cp.Location)
                  .WithMany(l => l.ConcessionsPricings)
                  .HasForeignKey(cp => cp.LocationId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ── ConcessionsSalesItem: binary(16) FK, computed column ──
        modelBuilder.Entity<ConcessionsSalesItem>(entity =>
        {
            entity.Property(si => si.ItemId).HasColumnType("binary(16)");
            entity.Property(si => si.LineTotal)
                  .HasColumnType("decimal(10,2)")
                  .ValueGeneratedOnAddOrUpdate();

            entity.HasOne(si => si.ConcessionsSale)
                  .WithMany(s => s.SalesItems)
                  .HasForeignKey(si => si.SaleId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(si => si.Inventory)
                  .WithMany(i => i.ConcessionsSalesItems)
                  .HasForeignKey(si => si.ItemId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ── ConcessionsOrder: binary(16) FKs ──
        modelBuilder.Entity<ConcessionsOrder>(entity =>
        {
            entity.Property(co => co.VendorId).HasColumnType("binary(16)");
            entity.Property(co => co.ItemId).HasColumnType("binary(16)");

            entity.HasOne(co => co.Vendor)
                  .WithMany(v => v.ConcessionsOrders)
                  .HasForeignKey(co => co.VendorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(co => co.Inventory)
                  .WithMany(i => i.ConcessionsOrders)
                  .HasForeignKey(co => co.ItemId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(co => co.Location)
                  .WithMany(l => l.ConcessionsOrders)
                  .HasForeignKey(co => co.LocationId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ── ScheduledShow relationships ──
        modelBuilder.Entity<ScheduledShow>(entity =>
        {
            entity.HasOne(ss => ss.Movie)
                  .WithMany(m => m.ScheduledShows)
                  .HasForeignKey(ss => ss.MovieId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ss => ss.Location)
                  .WithMany(l => l.ScheduledShows)
                  .HasForeignKey(ss => ss.LocationId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ss => ss.TheaterRoom)
                  .WithMany(r => r.ScheduledShows)
                  .HasForeignKey(ss => ss.RoomId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Ticket relationships ──
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasOne(t => t.ScheduledShow)
                  .WithMany(ss => ss.Tickets)
                  .HasForeignKey(t => t.ShowId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(t => t.PriceTier)
                  .WithMany(pt => pt.Tickets)
                  .HasForeignKey(t => t.PriceTierId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Customer)
                  .WithMany(c => c.Tickets)
                  .HasForeignKey(t => t.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ── ConcessionsSale relationships ──
        modelBuilder.Entity<ConcessionsSale>(entity =>
        {
            entity.HasOne(cs => cs.Location)
                  .WithMany(l => l.ConcessionsSales)
                  .HasForeignKey(cs => cs.LocationId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(cs => cs.Employee)
                  .WithMany(e => e.ConcessionsSales)
                  .HasForeignKey(cs => cs.EmployeeId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ── Employee / Role ──
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasOne(e => e.Role)
                  .WithMany(r => r.Employees)
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
