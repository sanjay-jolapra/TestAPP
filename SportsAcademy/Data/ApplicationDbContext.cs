using Microsoft.EntityFrameworkCore;
using SportsAcademy.Models;
using SportsAcademy.Models.Auth;

namespace SportsAcademy.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // Existing domain tables
    public DbSet<Batch> Batches => Set<Batch>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<FeePayment> FeePayments => Set<FeePayment>();
    public DbSet<FitnessTest> FitnessTests => Set<FitnessTest>();
    public DbSet<FitnessTestResult> FitnessTestResults => Set<FitnessTestResult>();
    public DbSet<MovementScreeningTest> MovementScreeningTests => Set<MovementScreeningTest>();
    public DbSet<MovementScreeningResult> MovementScreeningResults => Set<MovementScreeningResult>();
    public DbSet<SkillGroup> SkillGroups => Set<SkillGroup>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<SkillAssessmentResult> SkillAssessmentResults => Set<SkillAssessmentResult>();
    public DbSet<SCGroup> SCGroups => Set<SCGroup>();
    public DbSet<SCExercise> SCExercises => Set<SCExercise>();
    public DbSet<SCAssessmentResult> SCAssessmentResults => Set<SCAssessmentResult>();
    public DbSet<MentalToughnessTest> MentalToughnessTests => Set<MentalToughnessTest>();
    public DbSet<MentalToughnessResult> MentalToughnessResults => Set<MentalToughnessResult>();
    public DbSet<TeamworkTest> TeamworkTests => Set<TeamworkTest>();
    public DbSet<TeamworkResult> TeamworkResults => Set<TeamworkResult>();
    public DbSet<InjuryRecord> InjuryRecords => Set<InjuryRecord>();
    public DbSet<NutritionLog> NutritionLogs => Set<NutritionLog>();
    public DbSet<NutritionGoal> NutritionGoals => Set<NutritionGoal>();

    // Auth tables
    public DbSet<AppRole> AppRoles => Set<AppRole>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<AppModule> AppModules => Set<AppModule>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Auth relationships
        modelBuilder.Entity<AppUser>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.Permissions)
            .HasForeignKey(rp => rp.RoleId);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Module)
            .WithMany(m => m.RolePermissions)
            .HasForeignKey(rp => rp.ModuleId);

        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // ── Existing seed data ──────────────────────────────────────

        modelBuilder.Entity<Batch>().HasData(
            new Batch { Id = 1, Name = "Morning Batch", StartTime = new TimeSpan(6, 0, 0), EndTime = new TimeSpan(8, 0, 0) },
            new Batch { Id = 2, Name = "Evening Batch", StartTime = new TimeSpan(16, 0, 0), EndTime = new TimeSpan(18, 0, 0) },
            new Batch { Id = 3, Name = "Weekend Batch", StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(11, 0, 0) }
        );

        modelBuilder.Entity<FitnessTest>().HasData(
            new FitnessTest { Id = 1, Name = "20m Sprint", MeasurementUnit = "seconds", Measures = "Speed", Description = "Sprint over 20 meters" },
            new FitnessTest { Id = 2, Name = "5-10-5 Shuttle", MeasurementUnit = "seconds", Measures = "Agility & Change of Direction", Description = "Pro agility shuttle run" },
            new FitnessTest { Id = 3, Name = "Vertical Jump", MeasurementUnit = "cm", Measures = "Power & Explosiveness", Description = "Standing vertical jump" }
        );

        modelBuilder.Entity<MovementScreeningTest>().HasData(
            new MovementScreeningTest { Id = 1, Name = "Deep Squat", MeasurementUnit = "reps", Measures = "Mobility & Stability", Description = "Deep squat movement pattern" },
            new MovementScreeningTest { Id = 2, Name = "Hurdle Step", MeasurementUnit = "mins", Measures = "Hip Mobility", Description = "Hurdle step assessment" },
            new MovementScreeningTest { Id = 3, Name = "Inline Lunge", MeasurementUnit = "reps", Measures = "Balance & Coordination" }
        );

        modelBuilder.Entity<SkillGroup>().HasData(
            new SkillGroup { Id = 1, Name = "Batting" },
            new SkillGroup { Id = 2, Name = "Bowling" },
            new SkillGroup { Id = 3, Name = "Fielding" }
        );

        modelBuilder.Entity<Skill>().HasData(
            new Skill { Id = 1, SkillGroupId = 1, Name = "Batting Stance", MeasurementUnit = "Score (1-10)", Measures = "Technique" },
            new Skill { Id = 2, SkillGroupId = 1, Name = "Grip", MeasurementUnit = "Score (1-10)", Measures = "Technique" },
            new Skill { Id = 3, SkillGroupId = 1, Name = "Backlift", MeasurementUnit = "Score (1-10)", Measures = "Technique" },
            new Skill { Id = 4, SkillGroupId = 2, Name = "Run-up", MeasurementUnit = "Score (1-10)", Measures = "Technique" },
            new Skill { Id = 5, SkillGroupId = 2, Name = "Delivery", MeasurementUnit = "Score (1-10)", Measures = "Technique" }
        );

        modelBuilder.Entity<SCGroup>().HasData(
            new SCGroup { Id = 1, Name = "Warm-up" },
            new SCGroup { Id = 2, Name = "Strength" },
            new SCGroup { Id = 3, Name = "Conditioning" }
        );

        modelBuilder.Entity<SCExercise>().HasData(
            new SCExercise { Id = 1, SCGroupId = 1, Name = "Jogging", MeasurementUnit = "minutes", Measures = "Endurance" },
            new SCExercise { Id = 2, SCGroupId = 1, Name = "Mobility Drills", MeasurementUnit = "minutes", Measures = "Flexibility" },
            new SCExercise { Id = 3, SCGroupId = 1, Name = "Dynamic Stretching", MeasurementUnit = "minutes", Measures = "Flexibility" },
            new SCExercise { Id = 4, SCGroupId = 2, Name = "Push-ups", MeasurementUnit = "reps", Measures = "Upper Body Strength" },
            new SCExercise { Id = 5, SCGroupId = 2, Name = "Squats", MeasurementUnit = "reps", Measures = "Leg Strength" }
        );

        modelBuilder.Entity<MentalToughnessTest>().HasData(
            new MentalToughnessTest { Id = 1, Name = "Confidence", MeasurementUnit = "Score (1-10)", Measures = "Self-belief", MaxScore = 10 },
            new MentalToughnessTest { Id = 2, Name = "Concentration", MeasurementUnit = "Score (1-10)", Measures = "Focus", MaxScore = 10 },
            new MentalToughnessTest { Id = 3, Name = "Decision Making", MeasurementUnit = "Score (1-10)", Measures = "Cognitive", MaxScore = 10 },
            new MentalToughnessTest { Id = 4, Name = "Resilience", MeasurementUnit = "Score (1-10)", Measures = "Mental Strength", MaxScore = 10 }
        );

        modelBuilder.Entity<TeamworkTest>().HasData(
            new TeamworkTest { Id = 1, Name = "Communication", MeasurementUnit = "Score (1-10)", Measures = "Team Communication", MaxScore = 10 },
            new TeamworkTest { Id = 2, Name = "Leadership", MeasurementUnit = "Score (1-10)", Measures = "Leadership Quality", MaxScore = 10 },
            new TeamworkTest { Id = 3, Name = "Cooperation", MeasurementUnit = "Score (1-10)", Measures = "Team Cooperation", MaxScore = 10 },
            new TeamworkTest { Id = 4, Name = "Sportsmanship", MeasurementUnit = "Score (1-10)", Measures = "Fair Play", MaxScore = 10 }
        );
    }
}
