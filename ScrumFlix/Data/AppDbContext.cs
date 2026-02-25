using Microsoft.EntityFrameworkCore;
using ScrumFlix.Models;

namespace ScrumFlix.Data;

public class AppDbContext : DbContext
{
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<TheaterScreen> Screens => Set<TheaterScreen>();
    public DbSet<Showtime> ShowTimes => Set<Showtime>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite($"Data Source={AppPaths.DatabasePath}");
    }
}
