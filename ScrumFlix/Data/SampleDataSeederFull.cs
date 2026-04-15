/*
 * File: /ScrumFlix/Data/SampleDataSeederFull.cs
 * Description: Seeds the database with initial theater locations, rooms, movies, price tiers,
 *              vendors, customers, employees, inventory, pricing, shows, and concession data.
 */

namespace ScrumFlix.Data;

/// <summary>
/// Static seeder class that populates the database with representative sample data on first run.
/// Checks for existing data before seeding to prevent duplicate entries.
/// </summary>
public static class SampleDataSeederFull
{
    /// <summary>
    /// Seeds all tables with sample data. Returns immediately if data already exists.
    /// </summary>
    /// <param name="db">The application database context to seed.</param>
    public static void Seed(AppDbContext db)
    {
        // ── Employee Roles ─────────────────────────────────────────────────────
        // Keyed on RoleName — each role name must be unique.
        var existingRoleNames = db.EmployeeRoles.Select(r => r.RoleName).ToHashSet();
        var roleCandidates = new List<EmployeeRole>
        {
            new() { RoleName = "Theater Manager",      RoleDescription = "Oversees operations, staff, and performance" },
            new() { RoleName = "Box Office Associate", RoleDescription = "Sells tickets and assists customers" },
            new() { RoleName = "Concessions Associate",RoleDescription = "Prepares and sells food and beverages" },
            new() { RoleName = "Usher",                RoleDescription = "Checks tickets and assists guests in theaters" },
            new() { RoleName = "Facilities Associate", RoleDescription = "Maintains cleanliness and basic upkeep of building" }
        };
        var newRoles = roleCandidates.Where(r => !existingRoleNames.Contains(r.RoleName)).ToList();
        if (newRoles.Any()) { db.EmployeeRoles.AddRange(newRoles); db.SaveChanges(); }

        var roles = db.EmployeeRoles.ToList();

        // ── Locations ──────────────────────────────────────────────────────────
        // Keyed on LocationName — each location name must be unique.
        var existingLocationNames = db.Locations.Select(l => l.LocationName).ToHashSet();
        var locationCandidates = new List<Location>
        {
            new() { LocationName = "Dallas Central",        LocationAddress = "123 Cinema Way, Dallas, TX",           IsActive = true },
            new() { LocationName = "Dallas North",          LocationAddress = "4800 Preston Rd, Dallas, TX",           IsActive = true },
            new() { LocationName = "Dallas South",          LocationAddress = "2100 Camp Wisdom Rd, Dallas, TX",       IsActive = true },
            new() { LocationName = "Fort Worth West",       LocationAddress = "1000 West 7th St, Fort Worth, TX",      IsActive = true },
            new() { LocationName = "Plano Legacy",          LocationAddress = "7300 Lone Star Dr, Plano, TX",          IsActive = true },
            new() { LocationName = "Arlington Parks",       LocationAddress = "850 E Randol Mill Rd, Arlington, TX",   IsActive = true },
            new() { LocationName = "Irving Valley Ranch",   LocationAddress = "4000 N Belt Line Rd, Irving, TX",       IsActive = true },
            new() { LocationName = "Frisco Star",           LocationAddress = "2601 Preston Rd, Frisco, TX",           IsActive = true },
            new() { LocationName = "Garland Commons",       LocationAddress = "4100 Lavon Dr, Garland, TX",            IsActive = true },
            new() { LocationName = "Mesquite Town East",    LocationAddress = "1220 Town East Blvd, Mesquite, TX",     IsActive = true }
        };
        var newLocations = locationCandidates.Where(l => !existingLocationNames.Contains(l.LocationName)).ToList();
        if (newLocations.Any()) { db.Locations.AddRange(newLocations); db.SaveChanges(); }

        var locations = db.Locations.ToList();

        // ── Theater Rooms ──────────────────────────────────────────────────────
        // Keyed on (LocationId, RoomName) — a room name must be unique within a location.
        var existingRoomKeys = db.TheaterRooms
            .Select(r => new { r.LocationId, r.RoomName })
            .ToHashSet();

        var roomCandidates = new List<TheaterRoom>();
        var screenCounts   = new[] { 8, 10, 7, 12, 9, 11, 6, 10, 8, 9 };
        for (int i = 0; i < locations.Count; i++)
            for (int s = 1; s <= screenCounts[i]; s++)
            {
                var name = s == 1 && screenCounts[i] >= 10 ? $"IMAX {s}" : $"Screen {s}";
                if (!existingRoomKeys.Any(k => k.LocationId == locations[i].LocationId && k.RoomName == name))
                    roomCandidates.Add(new TheaterRoom
                    {
                        LocationId      = locations[i].LocationId,
                        RoomName        = name,
                        SeatingCapacity = s == 1 && screenCounts[i] >= 10 ? 220 : 88 + (s * 10),
                        IsActive        = true
                    });
            }
        if (roomCandidates.Any()) db.TheaterRooms.AddRange(roomCandidates);
        db.SaveChanges();

        var rooms = db.TheaterRooms.ToList();

        // ── Movies ─────────────────────────────────────────────────────────────
        // Keyed on MovieName — each title must be unique.
        var existingMovieNames = db.Movies.Select(m => m.MovieName).ToHashSet();
        var movieCandidates = new List<Movie>
        {
            new() { MovieName = "The Last Orbit",     Genre = "Sci-Fi",     MpaRating = "PG-13", RunTime = 126 },
            new() { MovieName = "Glass Harbor",       Genre = "Thriller",   MpaRating = "R",     RunTime = 109 },
            new() { MovieName = "Neon Riders",        Genre = "Action",     MpaRating = "PG-13", RunTime = 118 },
            new() { MovieName = "Moonlit Harbor",     Genre = "Drama",      MpaRating = "PG",    RunTime = 114 },
            new() { MovieName = "Final Tempo",        Genre = "Musical",    MpaRating = "PG",    RunTime = 121 },
            new() { MovieName = "Signal Lost",        Genre = "Mystery",    MpaRating = "PG-13", RunTime = 112 },
            new() { MovieName = "Copper Skies",       Genre = "Adventure",  MpaRating = "PG-13", RunTime = 123 },
            new() { MovieName = "After the Ashes",    Genre = "Drama",      MpaRating = "R",     RunTime = 132 },
            new() { MovieName = "Pixel Frontier",     Genre = "Animation",  MpaRating = "PG",    RunTime = 97  },
            new() { MovieName = "Midnight Circuit",   Genre = "Action",     MpaRating = "PG-13", RunTime = 115 },
            new() { MovieName = "Hollow Creek",       Genre = "Horror",     MpaRating = "R",     RunTime = 104 },
            new() { MovieName = "Sunset Protocol",    Genre = "Sci-Fi",     MpaRating = "PG-13", RunTime = 128 },
            new() { MovieName = "Paper Crown",        Genre = "Romance",    MpaRating = "PG-13", RunTime = 111 },
            new() { MovieName = "Frozen Signal",      Genre = "Thriller",   MpaRating = "PG-13", RunTime = 107 },
            new() { MovieName = "Wild Line",          Genre = "Western",    MpaRating = "PG-13", RunTime = 119 },
            new() { MovieName = "Northstar Run",      Genre = "Adventure",  MpaRating = "PG",    RunTime = 116 },
            new() { MovieName = "Echo Garden",        Genre = "Fantasy",    MpaRating = "PG",    RunTime = 124 },
            new() { MovieName = "Breakwater",         Genre = "Action",     MpaRating = "PG-13", RunTime = 110 },
            new() { MovieName = "The Long Frame",     Genre = "Crime",      MpaRating = "R",     RunTime = 129 },
            new() { MovieName = "Velvet Avenue",      Genre = "Comedy",     MpaRating = "PG-13", RunTime = 102 }
        };
        var newMovies = movieCandidates.Where(m => !existingMovieNames.Contains(m.MovieName)).ToList();
        if (newMovies.Any()) db.Movies.AddRange(newMovies);

        var movies = db.Movies.ToList();

        // ── Price Tiers ────────────────────────────────────────────────────────
        // Keyed on (CategoryName, FormatType) — the combination must be unique.
        var existingTierKeys = db.PriceTiers
            .Select(t => new { t.CategoryName, t.FormatType })
            .ToList();

        var priceTierCandidates = new[]
        {
            new PriceTier { CategoryName = "Adult Standard",      FormatType = "2D",   Price = 16.00m, IsActive = true, Description = "Standard adult admission" },
            new PriceTier { CategoryName = "Child Standard",      FormatType = "2D",   Price = 9.00m,  IsActive = true, Description = "Children ages 2-12" },
            new PriceTier { CategoryName = "Senior Standard",     FormatType = "2D",   Price = 11.00m, IsActive = true, Description = "Seniors ages 60+" },
            new PriceTier { CategoryName = "Adult IMAX",          FormatType = "IMAX", Price = 21.00m, IsActive = true, Description = "Premium IMAX experience" },
            new PriceTier { CategoryName = "Matinee Special",     FormatType = "2D",   Price = 9.00m,  IsActive = true, Description = "All shows before 2:00 PM" },
            new PriceTier { CategoryName = "Premium Large Format",FormatType = "PLF",  Price = 19.00m, IsActive = true, Description = "Premium auditorium pricing" }
        };
        var newTiers = priceTierCandidates
            .Where(t => !existingTierKeys.Any(k => k.CategoryName == t.CategoryName && k.FormatType == t.FormatType))
            .ToList();
        if (newTiers.Any()) db.PriceTiers.AddRange(newTiers);
        db.SaveChanges();

        // ── Scheduled Shows ────────────────────────────────────────────────────
        // Load existing (LocationId, RoomId, StartDateTime) triples from DB so the
        // in-memory tracking sets start accurate, not empty.
        var usedShowSlots = db.ScheduledShows
            .Select(s => new { s.LocationId, s.RoomId, s.StartDateTime })
            .AsEnumerable()
            .Select(s => (s.LocationId, s.RoomId, s.StartDateTime))
            .ToHashSet<(int LocationId, int RoomId, DateTime Start)>();

        // Also seed per-location used-start-times from DB so new slots never
        // collide with already-persisted showtimes at the same location.
        var usedStartsByLocation = db.ScheduledShows
            .Select(s => new { s.LocationId, s.StartDateTime })
            .AsEnumerable()
            .GroupBy(s => s.LocationId)
            .ToDictionary(g => g.Key, g => g.Select(s => s.StartDateTime).ToHashSet());

        var showRows = new List<ScheduledShow>();

        // Base showtime offsets (hours from midnight). Each slot is spread far
        // enough apart that shows at the same location are always distinct.
        var baseHourSlots = new[] { 10, 13, 16, 19, 22 };

        int showIndex = 0;
        for (int i = 0; i < 140; i++)
        {
            var loc        = locations[i % locations.Count];
            var localRooms = rooms.Where(r => r.LocationId == loc.LocationId).ToList();
            var room       = localRooms[i % localRooms.Count];
            var movie      = movies[i % movies.Count];

            // Ensure the per-location tracking set exists.
            if (!usedStartsByLocation.ContainsKey(loc.LocationId))
                usedStartsByLocation[loc.LocationId] = new HashSet<DateTime>();

            // Find the next available (day, hour) slot that is unique for this
            // location AND unique for the specific (location, room) combination.
            DateTime start    = DateTime.MinValue;
            bool     slotFound = false;

            for (int dayOffset = 0; dayOffset < 14 && !slotFound; dayOffset++)
            {
                foreach (var hour in baseHourSlots)
                {
                    var candidate = DateTime.Today.AddDays(dayOffset).AddHours(hour);

                    // Skip if this start time is already used anywhere at this location.
                    if (usedStartsByLocation[loc.LocationId].Contains(candidate))
                        continue;

                    // Skip if this exact (location, room, start) triple already exists.
                    if (usedShowSlots.Contains((loc.LocationId, room.RoomId, candidate)))
                        continue;

                    start     = candidate;
                    slotFound = true;
                    break;
                }
            }

            // If no slot was found within the search window, skip this iteration
            // rather than insert a duplicate or an invalid row.
            if (!slotFound)
                continue;

            // Record the chosen slot so later iterations avoid it.
            usedShowSlots.Add((loc.LocationId, room.RoomId, start));
            usedStartsByLocation[loc.LocationId].Add(start);

            // Clamp TicketsSold so AvailableSeats is never negative.
            int capacity    = room.SeatingCapacity;
            int rawSold     = 18 + ((i * 9) % 170);
            int ticketsSold = Math.Min(rawSold, capacity);           // ≤ capacity
            int available   = Math.Max(capacity - ticketsSold, 0);   // always ≥ 0

            showRows.Add(new ScheduledShow
            {
                MovieId        = movie.MovieId,
                LocationId     = loc.LocationId,
                RoomId         = room.RoomId,
                ShowDate       = start.Date,
                StartDateTime  = start,
                EndDateTime    = start.AddMinutes(movie.RunTime + 22),
                IsActive       = true,
                TicketsSold    = ticketsSold
            });

            showIndex++;
        }
        db.ScheduledShows.AddRange(showRows);

        // ── Vendors ────────────────────────────────────────────────────────────
        // Keyed on VendorName — each vendor name must be unique.
        var existingVendorNames = db.Vendors.Select(v => v.VendorName).ToHashSet();
        var vendorCandidates = new List<Vendor>
        {
            new() { VendorId = Guid.NewGuid().ToByteArray(), VendorName = "Lone Star Concessions",    VendorAddress = "480 Supply Lane",     VendorCity = "Dallas",  VendorState = "TX", VendorZipcode = 75201, VendorPhone = "2145551200", VendorContactName = "Avery Cole",   VendorEmail = "orders@lonestarconcessions.com" },
            new() { VendorId = Guid.NewGuid().ToByteArray(), VendorName = "Metro Snack Supply",       VendorAddress = "1200 Warehouse Dr",   VendorCity = "Dallas",  VendorState = "TX", VendorZipcode = 75202, VendorPhone = "2145551300", VendorContactName = "Jordan Lee",   VendorEmail = "sales@metrosnack.com" },
            new() { VendorId = Guid.NewGuid().ToByteArray(), VendorName = "Texas Beverage Partners",  VendorAddress = "900 Bottler Pkwy",    VendorCity = "Irving",  VendorState = "TX", VendorZipcode = 75038, VendorPhone = "9725558800", VendorContactName = "Morgan Chen",  VendorEmail = "support@txbevpartners.com" }
        };
        var newVendors = vendorCandidates.Where(v => !existingVendorNames.Contains(v.VendorName)).ToList();
        if (newVendors.Any()) db.Vendors.AddRange(newVendors);
        db.SaveChanges();

        var vendors = db.Vendors.ToList();

        // ── Customers ──────────────────────────────────────────────────────────
        // Keyed on CustomerEmail — each email address must be unique.
        var existingCustomerEmails = db.Customers.Select(c => c.CustomerEmail).ToHashSet();
        var customerCandidates = new List<Customer>
        {
            new() { FirstName = "Emma",   LastName = "Reed",    CustomerEmail = "emma.reed@example.com",    CustomerPhone = "2145550101", CustomerDob = new DateTime(1994, 4, 12), CustomerCategory = "Adult"   },
            new() { FirstName = "Noah",   LastName = "Carter",  CustomerEmail = "noah.carter@example.com",  CustomerPhone = "2145550102", CustomerDob = new DateTime(1988, 9,  3), CustomerCategory = "Adult"   },
            new() { FirstName = "Ava",    LastName = "Nguyen",  CustomerEmail = "ava.nguyen@example.com",   CustomerPhone = "2145550103", CustomerDob = new DateTime(2014, 2, 16), CustomerCategory = "Child"   },
            new() { FirstName = "Liam",   LastName = "Patel",   CustomerEmail = "liam.patel@example.com",   CustomerPhone = "2145550104", CustomerDob = new DateTime(1962, 7, 21), CustomerCategory = "Senior"  },
            new() { FirstName = "Sophia", LastName = "Diaz",    CustomerEmail = "sophia.diaz@example.com",  CustomerPhone = "2145550105", CustomerDob = new DateTime(2001,11,  9), CustomerCategory = "Matinee" },
            new() { FirstName = "Mason",  LastName = "Brooks",  CustomerEmail = "mason.brooks@example.com", CustomerPhone = "2145550106", CustomerDob = new DateTime(1999, 1, 28), CustomerCategory = "Adult"   }
        };
        var newCustomers = customerCandidates.Where(c => !existingCustomerEmails.Contains(c.CustomerEmail)).ToList();
        if (newCustomers.Any()) db.Customers.AddRange(newCustomers);

        // ── Employees ──────────────────────────────────────────────────────────
        // Keyed on EmployeeEmail — each work email must be unique.
        var existingEmployeeEmails = db.Employees.Select(e => e.EmployeeEmail).ToHashSet();
        var employeeCandidates = new List<Employee>
        {
            new() { FirstName = "Grace",   LastName = "Howard",   EmployeeStartDate = DateTime.Today.AddYears(-2).Date,    EmployeeDob = new DateTime(1987, 5, 14), EmployeePhone = "2145550201", EmployeeEmail = "grace.howard@theater.com",   EmployeeCity = "Dallas",    EmployeeState = "TX", EmployeeZipcode = "75201", RoleId = roles.First(r => r.RoleName == "Theater Manager").RoleId      },
            new() { FirstName = "Brandon", LastName = "Cole",     EmployeeStartDate = DateTime.Today.AddYears(-1).Date,    EmployeeDob = new DateTime(1995, 8, 24), EmployeePhone = "2145550202", EmployeeEmail = "brandon.cole@theater.com",   EmployeeCity = "Dallas",    EmployeeState = "TX", EmployeeZipcode = "75202", RoleId = roles.First(r => r.RoleName == "Box Office Associate").RoleId  },
            new() { FirstName = "Tina",    LastName = "Martinez", EmployeeStartDate = DateTime.Today.AddMonths(-9).Date,   EmployeeDob = new DateTime(1998, 3, 11), EmployeePhone = "2145550203", EmployeeEmail = "tina.martinez@theater.com",  EmployeeCity = "Arlington", EmployeeState = "TX", EmployeeZipcode = "76010", RoleId = roles.First(r => r.RoleName == "Concessions Associate").RoleId },
            new() { FirstName = "Derek",   LastName = "Johnson",  EmployeeStartDate = DateTime.Today.AddMonths(-14).Date,  EmployeeDob = new DateTime(1992,10,  5), EmployeePhone = "2145550204", EmployeeEmail = "derek.johnson@theater.com",  EmployeeCity = "Irving",    EmployeeState = "TX", EmployeeZipcode = "75061", RoleId = roles.First(r => r.RoleName == "Usher").RoleId                 },
            new() { FirstName = "Nina",    LastName = "Singh",    EmployeeStartDate = DateTime.Today.AddMonths(-6).Date,   EmployeeDob = new DateTime(2000, 6, 19), EmployeePhone = "2145550205", EmployeeEmail = "nina.singh@theater.com",     EmployeeCity = "Plano",     EmployeeState = "TX", EmployeeZipcode = "75024", RoleId = roles.First(r => r.RoleName == "Facilities Associate").RoleId  }
        };
        var newEmployees = employeeCandidates.Where(e => !existingEmployeeEmails.Contains(e.EmployeeEmail)).ToList();
        if (newEmployees.Any()) db.Employees.AddRange(newEmployees);
        db.SaveChanges();

        var employees = db.Employees.ToList();

        // ── Inventory ──────────────────────────────────────────────────────────
        // Keyed on (LocationId, ItemName) — each product name must be unique per location.
        var existingInventoryKeys = db.Inventory
            .Select(inv => new { inv.LocationId, inv.ItemName })
            .ToList();

        var productTemplates = new[]
        {
            new { Name = "Small Popcorn",         Cost = 1.10m,  Price = 5.99m  },
            new { Name = "Medium Popcorn",         Cost = 1.45m,  Price = 7.49m  },
            new { Name = "Large Popcorn",          Cost = 1.85m,  Price = 8.99m  },
            new { Name = "Small Fountain Drink",   Cost = 0.55m,  Price = 4.99m  },
            new { Name = "Large Fountain Drink",   Cost = 0.75m,  Price = 6.29m  },
            new { Name = "Icee",                   Cost = 0.82m,  Price = 6.49m  },
            new { Name = "Candy Box",              Cost = 0.78m,  Price = 4.79m  },
            new { Name = "Nachos",                 Cost = 1.35m,  Price = 6.99m  },
            new { Name = "Pretzel",                Cost = 1.05m,  Price = 5.99m  },
            new { Name = "Hot Dog",                Cost = 1.40m,  Price = 6.49m  },
            new { Name = "Bottled Water",          Cost = 0.55m,  Price = 3.99m  },
            new { Name = "Kids Combo",             Cost = 2.10m,  Price = 8.49m  },
            new { Name = "Popcorn + Drink Combo",  Cost = 2.40m,  Price = 11.99m },
            new { Name = "Nachos + Drink Combo",   Cost = 2.65m,  Price = 12.49m },
            new { Name = "Extra Butter",           Cost = 0.15m,  Price = 0.99m  },
            new { Name = "Cheese Cup",             Cost = 0.42m,  Price = 1.49m  },
            new { Name = "Jalapenos",              Cost = 0.20m,  Price = 0.99m  },
            new { Name = "Souvenir Cup Upgrade",   Cost = 0.65m,  Price = 3.49m  }
        };

        var newInventoryRows = new List<Inventory>();
        foreach (var loc in locations)
            for (int i = 0; i < productTemplates.Length; i++)
            {
                if (existingInventoryKeys.Any(k => k.LocationId == loc.LocationId && k.ItemName == productTemplates[i].Name))
                    continue;
                newInventoryRows.Add(new Inventory
                {
                    ItemName     = productTemplates[i].Name,
                    UnitCost     = productTemplates[i].Cost,
                    ItemQuantity = 80 + ((loc.LocationId + i) % 12) * 20,
                    VendorId     = vendors[i % vendors.Count].VendorId,
                    LocationId   = loc.LocationId
                });
            }
        if (newInventoryRows.Any()) db.Inventory.AddRange(newInventoryRows);
        db.SaveChanges();

        var inventoryRows = db.Inventory.ToList();

        // ── Concessions Pricing ────────────────────────────────────────────────
        // Keyed on (ItemId, LocationId) — one active price per item per location.
        var existingPricingKeys = db.ConcessionsPricing
            .Select(p => new { p.ItemId, p.LocationId })
            .AsEnumerable()
            .Select(p => (p.ItemId, p.LocationId))
            .ToHashSet();

        var newPricingRows = new List<ConcessionsPricing>();
        foreach (var inv in inventoryRows)
        {
            if (existingPricingKeys.Contains((inv.ItemId, inv.LocationId)))
                continue;
            var loc  = locations.First(l => l.LocationId == inv.LocationId);
            var mult = loc.LocationName.Contains("Plano") || loc.LocationName.Contains("Frisco")        ? 1.07m
                     : loc.LocationName.Contains("Fort Worth") || loc.LocationName.Contains("Mesquite") ? 0.97m
                     : 1.00m;
            var tpl  = productTemplates.First(p => p.Name == inv.ItemName);
            newPricingRows.Add(new ConcessionsPricing
            {
                ItemId         = inv.ItemId,
                LocationId     = inv.LocationId,
                Price          = Math.Round(tpl.Price * mult, 2),
                EffectiveStart = DateTime.Today,
                IsActive       = true
            });
        }
        if (newPricingRows.Any()) db.ConcessionsPricing.AddRange(newPricingRows);

        // ── Concessions Sales ──────────────────────────────────────────────────
        // Keyed on (LocationId, SaleDatetime, EmployeeId) — no two identical sale headers.
        var existingSaleKeys = db.ConcessionsSales
            .Select(s => new { s.LocationId, s.SaleDatetime, s.EmployeeId })
            .ToList();

        var newSales = new List<ConcessionsSale>();
        for (int i = 0; i < 12; i++)
        {
            var saleLocId  = locations[i % locations.Count].LocationId;
            var saleTime   = DateTime.Today.AddHours(12 + i);
            var saleEmpId  = employees[i % employees.Count].EmployeeId;
            if (existingSaleKeys.Any(k => k.LocationId == saleLocId && k.SaleDatetime == saleTime && k.EmployeeId == saleEmpId))
                continue;
            newSales.Add(new ConcessionsSale
            {
                LocationId   = saleLocId,
                SaleDatetime = saleTime,
                EmployeeId   = saleEmpId,
                TotalAmount  = 0m
            });
        }
        if (newSales.Any()) db.ConcessionsSales.AddRange(newSales);
        db.SaveChanges();

        var sales        = db.ConcessionsSales.ToList();
        var pricingRows  = db.ConcessionsPricing.ToList();

        // ── Sale Items ─────────────────────────────────────────────────────────
        // Keyed on (SaleId, ItemId) — one line per item per sale.
        var existingSaleItemKeys = db.ConcessionsSalesItems
            .Select(si => new { si.SaleId, si.ItemId })
            .AsEnumerable()
            .Select(si => (si.SaleId, si.ItemId))
            .ToHashSet();

        var newSaleItems = new List<ConcessionsSalesItem>();
        for (int i = 0; i < sales.Count; i++)
        {
            var inv1 = inventoryRows[(i * 2)     % inventoryRows.Count];
            var inv2 = inventoryRows[(i * 2 + 5) % inventoryRows.Count];

            if (!existingSaleItemKeys.Contains((sales[i].SaleId, inv1.ItemId)))
                newSaleItems.Add(new ConcessionsSalesItem
                {
                    SaleId    = sales[i].SaleId,
                    ItemId    = inv1.ItemId,
                    Quantity  = 1,
                    UnitPrice = pricingRows.First(p => p.ItemId.SequenceEqual(inv1.ItemId) && p.LocationId == inv1.LocationId).Price
                });

            if (!existingSaleItemKeys.Contains((sales[i].SaleId, inv2.ItemId)))
                newSaleItems.Add(new ConcessionsSalesItem
                {
                    SaleId    = sales[i].SaleId,
                    ItemId    = inv2.ItemId,
                    Quantity  = 2,
                    UnitPrice = pricingRows.First(p => p.ItemId.SequenceEqual(inv2.ItemId) && p.LocationId == inv2.LocationId).Price
                });
        }
        if (newSaleItems.Any()) db.ConcessionsSalesItems.AddRange(newSaleItems);

        // ── Concessions Orders ─────────────────────────────────────────────────
        // Keyed on (VendorId, ItemId, LocationId, OrderDate) — one order per vendor/item/location/day.
        var existingOrderKeys = db.ConcessionsOrders
            .Select(o => new { o.VendorId, o.ItemId, o.LocationId, o.OrderDate })
            .AsEnumerable()
            .Select(o => (VendorId: Convert.ToBase64String(o.VendorId), ItemId: Convert.ToBase64String(o.ItemId), o.LocationId, o.OrderDate))
            .ToHashSet();

        var newOrders = new List<ConcessionsOrder>();
        for (int i = 0; i < 12; i++)
        {
            var vendor    = vendors[i % vendors.Count];
            var item      = inventoryRows[i];
            var location  = locations[i % locations.Count];
            var orderDate = DateTime.Today.AddDays(-i);
            var key       = (Convert.ToBase64String(vendor.VendorId), Convert.ToBase64String(item.ItemId), location.LocationId, orderDate);
            if (existingOrderKeys.Contains(key))
                continue;
            newOrders.Add(new ConcessionsOrder
            {
                OrderDate         = orderDate,
                VendorId          = vendor.VendorId,
                ItemId            = item.ItemId,
                OrderQuantity     = 40 + (i * 5),
                UnitPrice         = item.UnitCost,
                LocationId        = location.LocationId,
                OrderReceived     = true,
                OrderReceivedDate = orderDate.AddHours(4)
            });
        }
        if (newOrders.Any()) db.ConcessionsOrders.AddRange(newOrders);
        db.SaveChanges();
    }
}
