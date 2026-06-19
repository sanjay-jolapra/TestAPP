using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Models.Auth;
using SportsAcademy.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=sportsacademy.db"));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddMemoryCache();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

var app = builder.Build();

// Initialise DB and seed auth data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
    await SeedAuthDataAsync(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// ── Auth seed ──────────────────────────────────────────────────────────────

static async Task SeedAuthDataAsync(ApplicationDbContext db)
{
    if (!db.AppModules.Any())
    {
        db.AppModules.AddRange(
            new AppModule { Id =  1, Name = "Dashboard",            DisplayName = "Dashboard",               Icon = "fas fa-tachometer-alt",  Section = "Main",        SortOrder =  1 },
            new AppModule { Id =  2, Name = "Batches",              DisplayName = "Batches",                 Icon = "fas fa-layer-group",     Section = "Management",  SortOrder =  2 },
            new AppModule { Id =  3, Name = "Students",             DisplayName = "Students",                Icon = "fas fa-users",           Section = "Management",  SortOrder =  3 },
            new AppModule { Id =  4, Name = "Attendance",           DisplayName = "Attendance",              Icon = "fas fa-clipboard-check", Section = "Management",  SortOrder =  4 },
            new AppModule { Id =  5, Name = "Fees",                 DisplayName = "Fees",                    Icon = "fas fa-rupee-sign",      Section = "Management",  SortOrder =  5 },
            new AppModule { Id =  6, Name = "FitnessTests",         DisplayName = "Fitness Tests",           Icon = "fas fa-heartbeat",       Section = "Assessments", SortOrder =  6 },
            new AppModule { Id =  7, Name = "MovementScreening",    DisplayName = "Movement Screening",      Icon = "fas fa-walking",         Section = "Assessments", SortOrder =  7 },
            new AppModule { Id =  8, Name = "SkillDevelopment",     DisplayName = "Skill Development",       Icon = "fas fa-star",            Section = "Assessments", SortOrder =  8 },
            new AppModule { Id =  9, Name = "StrengthConditioning", DisplayName = "Strength & Conditioning", Icon = "fas fa-dumbbell",        Section = "Assessments", SortOrder =  9 },
            new AppModule { Id = 10, Name = "MentalToughness",      DisplayName = "Mental Toughness",        Icon = "fas fa-brain",           Section = "Assessments", SortOrder = 10 },
            new AppModule { Id = 11, Name = "Teamwork",             DisplayName = "Teamwork & Leadership",   Icon = "fas fa-handshake",       Section = "Assessments", SortOrder = 11 },
            new AppModule { Id = 12, Name = "InjuryTracking",       DisplayName = "Injury Tracking",         Icon = "fas fa-band-aid",        Section = "Health",      SortOrder = 12 },
            new AppModule { Id = 13, Name = "Nutrition",            DisplayName = "Nutrition",               Icon = "fas fa-apple-alt",       Section = "Health",      SortOrder = 13 },
            new AppModule { Id = 14, Name = "Performance",          DisplayName = "AI Performance Score",    Icon = "fas fa-chart-line",      Section = "Health",      SortOrder = 14 }
        );
        await db.SaveChangesAsync();
    }

    if (!db.AppRoles.Any())
    {
        db.AppRoles.AddRange(
            new AppRole { Id = 1, Name = "SuperAdmin", Description = "Full access to all modules and administration", IsActive = true, CreatedAt = DateTime.UtcNow },
            new AppRole { Id = 2, Name = "Coach",      Description = "Access to student data and all assessment modules", IsActive = true, CreatedAt = DateTime.UtcNow },
            new AppRole { Id = 3, Name = "FeeManager", Description = "Manages student fees and payment records",          IsActive = true, CreatedAt = DateTime.UtcNow },
            new AppRole { Id = 4, Name = "Viewer",     Description = "Read-only access to all modules",                   IsActive = true, CreatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();

        var allModuleIds = db.AppModules.Select(m => m.Id).ToList();

        // SuperAdmin – full CRUD on all modules
        db.RolePermissions.AddRange(allModuleIds.Select(mid => new RolePermission
        {
            RoleId = 1, ModuleId = mid,
            CanView = true, CanCreate = true, CanEdit = true, CanDelete = true
        }));

        // Coach – view+create+edit on all except Fees
        int[] coachModules = [1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14];
        db.RolePermissions.AddRange(coachModules.Select(mid => new RolePermission
        {
            RoleId = 2, ModuleId = mid,
            CanView = true, CanCreate = true, CanEdit = true, CanDelete = false
        }));

        // FeeManager – Dashboard + Students/Attendance (view) + Fees (full)
        db.RolePermissions.AddRange(new[]
        {
            new RolePermission { RoleId = 3, ModuleId =  1, CanView = true },
            new RolePermission { RoleId = 3, ModuleId =  3, CanView = true },
            new RolePermission { RoleId = 3, ModuleId =  4, CanView = true },
            new RolePermission { RoleId = 3, ModuleId =  5, CanView = true, CanCreate = true, CanEdit = true, CanDelete = true },
        });

        // Viewer – view-only on all modules
        db.RolePermissions.AddRange(allModuleIds.Select(mid => new RolePermission
        {
            RoleId = 4, ModuleId = mid, CanView = true
        }));

        await db.SaveChangesAsync();
    }

    if (!db.AppUsers.Any())
    {
        var authService = new AuthService(db);
        var salt = authService.GenerateSalt();
        db.AppUsers.Add(new AppUser
        {
            Username = "admin",
            Email = "admin@sportsacademy.com",
            FullName = "System Administrator",
            PasswordSalt = salt,
            PasswordHash = authService.HashPassword("Admin@123", salt),
            RoleId = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }
}
