/*
 * File: /ScrumFlix/Program.cs
 * Description: Application entry point. Configures services (EF Core / MySQL, session, cart,
 *              global filters), middleware pipeline, routing, and database seeding.
 */

using Microsoft.EntityFrameworkCore;
using ScrumFlix.Data;

var builder = WebApplication.CreateBuilder(args);

// ── Services ───────────────────────────────────────────────────────────────

// Entity Framework Core — Pomelo MySQL provider
var connectionString = builder.Configuration.GetConnectionString("MySQLConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found. " +
        "Run: dotnet user-secrets set \"ConnectionStrings:MySqlConnection\" \"Server=...\"");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information);
});

// Session support for the shopping cart
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Name = ".ScrumFlix.Session";
});

// HTTP context accessor — required by CartService
builder.Services.AddHttpContextAccessor();

// CartService — scoped per request
builder.Services.AddScoped<CartService>();

// CartCountFilter — injects cart count into ViewBag on every action
builder.Services.AddScoped<CartCountFilter>();

// MVC + Newtonsoft.Json + global CartCountFilter registration
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<CartCountFilter>();
})
.AddNewtonsoftJson(opts =>
    opts.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


var app = builder.Build();

// ── Middleware Pipeline ────────────────────────────────────────────────────

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// ── Areas (Admin scaffold) ─────────────────────────────────────────────────
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=AdminHome}/{action=AdminDashboard}/{id?}");

// ── Default Route ──────────────────────────────────────────────────────────
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=HomeDashboard}/{id?}");

// ── Database Seeding ───────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        db.Database.EnsureCreated();
        SampleDataSeederFull.Seed(db);
        logger.LogInformation("Database seeding complete.");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex,
            "Database seeding skipped or partially failed. " +
            "Ensure the database is reachable and credentials are set via User Secrets.");
    }
}

app.Run();
