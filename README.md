# ScrumFlix — Connection String Setup (User Secrets)

**NEVER commit real credentials to source control.**

Use ASP.NET Core User Secrets to store your Aiven MySQL connection string locally:

```bash
cd ScrumFlix
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=mysql-scrumtheater-scrumflix-theater.b.aivencloud.com;Port=YOUR_PORT;Database=defaultdb;User=API_1;Password=YOUR_PASSWORD;SslMode=Required;"
```

For deployment to somee.com, set the connection string via the somee.com control panel environment variables or web.config transformation — never in appsettings.json.

## Project Structure

```
ScrumFlix/
├── ScrumFlix.slnx
└── ScrumFlix/
    ├── Areas/Admin/          ← Scaffolded admin (future)
    ├── Controllers/          ← HomeController, MoviesController, etc.
    ├── Data/                 ← AppDbContext, CartService, SampleDataSeeder
    ├── Models/               ← All EF Core entity models
    ├── ViewModels/           ← Catalog, Detail, Booking, Cart VMs
    ├── Views/                ← Razor views (Home, Movies, Showtimes, etc.)
    ├── wwwroot/
    │   ├── css/scrumflix.css
    │   ├── js/scrumflix.js
    │   └── images/           ← Place logo.png and movie_theatre_background.png here
    ├── appsettings.json      ← Placeholder only (no real credentials)
    └── Program.cs
```

## Images

Copy your provided assets into `wwwroot/images/`:
- `logo.png`                    → `/wwwroot/images/logo.png`
- `movie_theatre_background.png` → `/wwwroot/images/movie_theatre_background.png`

## Build & Run

```bash
dotnet restore
dotnet run --project ScrumFlix
```
