using System.ComponentModel.DataAnnotations;

namespace SportsAcademy.Models;

public class RolePermission
{
    public int Id { get; set; }

    public int RoleId { get; set; }
    public Role? Role { get; set; }

    [Required, StringLength(100)]
    public string Module { get; set; } = "";

    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
}

// All available modules — single source of truth
public static class AppModules
{
    public const string Dashboard          = "Dashboard";
    public const string Batches            = "Batches";
    public const string Students           = "Students";
    public const string Attendance         = "Attendance";
    public const string Fees               = "Fees";
    public const string FitnessTests       = "FitnessTests";
    public const string MovementScreening  = "MovementScreening";
    public const string SkillDevelopment   = "SkillDevelopment";
    public const string StrengthConditioning = "StrengthConditioning";
    public const string MentalToughness    = "MentalToughness";
    public const string Teamwork           = "Teamwork";
    public const string InjuryTracking     = "InjuryTracking";
    public const string Nutrition          = "Nutrition";
    public const string Performance        = "Performance";
    public const string UserManagement     = "UserManagement";

    public static readonly string[] All =
    [
        Dashboard, Batches, Students, Attendance, Fees,
        FitnessTests, MovementScreening, SkillDevelopment,
        StrengthConditioning, MentalToughness, Teamwork,
        InjuryTracking, Nutrition, Performance, UserManagement
    ];

    public static readonly Dictionary<string, string> Labels = new()
    {
        [Dashboard]           = "Dashboard",
        [Batches]             = "Batches",
        [Students]            = "Students",
        [Attendance]          = "Attendance",
        [Fees]                = "Fees Management",
        [FitnessTests]        = "Fitness Tests",
        [MovementScreening]   = "Movement Screening",
        [SkillDevelopment]    = "Skill Development",
        [StrengthConditioning]= "Strength & Conditioning",
        [MentalToughness]     = "Mental Toughness",
        [Teamwork]            = "Teamwork & Leadership",
        [InjuryTracking]      = "Injury Tracking",
        [Nutrition]           = "Nutrition Tracking",
        [Performance]         = "AI Performance Score",
        [UserManagement]      = "User Management",
    };
}
