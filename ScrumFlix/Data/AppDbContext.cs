// Connects the program to the database

using Microsoft.EntityFrameworkCore;
using ScrumFlix.Models;
using System.Data;

namespace ScrumFlix.Data;

public class AppDbContext : DbContext
{
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<TheaterScreen> TheaterScreen => Set<TheaterScreen>();
    public DbSet<Showtime> Showtime => Set<Showtime>();
    public DbSet<Location> Location => Set<Location>();
    public DbSet<Ticket> Ticket { get; set; }
    public DbSet<User> Users => Set<User>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Role> Roles => Set<Role>();

    /* protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite($"Data Source={AppPaths.DatabasePath}");
    } < old sqlite connection*/

    // new aiven connection
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        var aiven =
            /*"Server=mysql-scrumtheater-scrumflix-theater.b.aivencloud.com;" +
            "Port=12031;" +
            "Database=defaultdb;" +
            "User=avnadmin;" +
            "Password=AVNS_qfxTTR9RIG_piTLOwLl;" +
            "SslMode=Required;";*/
            "Server=scrumflix12-project11923918414.k.aivencloud.com;" +
            "Port=28690;" +
            "Database=defaultdb;" +
            "User=avnadmin;" +
            "Password=AVNS_grUiTjBHE9kIGTMm7Iu;" +
            "SslMode=Required;";

        optionsBuilder.UseMySQL(aiven);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TheaterScreen>()
            .Property(t => t.Capacity)
            .HasDefaultValue(50);
    }
}
