// Connects the program to the database

using Microsoft.EntityFrameworkCore;
using ScrumFlix.Models;

namespace ScrumFlix.Data;

public class AppDbContext : DbContext
{
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<TheaterScreen> TheaterScreen => Set<TheaterScreen>();
    public DbSet<Showtime> ShowTimes => Set<Showtime>();
    public DbSet<Location> Location => Set<Location>();

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
            "Server=mysql-scrumtheater-scrumflix-theater.b.aivencloud.com;" +
            "Port=12031;" +
            "Database=defaultdb;" +
            "User=avnadmin;" +
            "Password=AVNS_qfxTTR9RIG_piTLOwLl;" +
            "SslMode=Required;";

        optionsBuilder.UseMySQL(aiven);
    }
}
