using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsAcademy.Models;

public class Student
{
    public int Id { get; set; }

    [Display(Name = "Student ID")]
    public string StudentCode { get; set; } = "";

    [Required, StringLength(150)]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = "";

    [Phone, StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(150)]
    [Display(Name = "Parent Name")]
    public string? ParentName { get; set; }

    [Phone, StringLength(20)]
    [Display(Name = "Parent Phone")]
    public string? ParentPhone { get; set; }

    public string? PhotoPath { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [StringLength(5)]
    [Display(Name = "Blood Group")]
    public string? BloodGroup { get; set; }

    [StringLength(200)]
    [Display(Name = "School / College")]
    public string? SchoolCollege { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(10)]
    [Display(Name = "Jersey Size")]
    public string? JerseySize { get; set; }

    [StringLength(12), MinLength(12)]
    [Display(Name = "Aadhar Card No.")]
    [RegularExpression(@"^\d{12}$", ErrorMessage = "Aadhar number must be exactly 12 digits.")]
    public string? AadharCardNo { get; set; }

    public string? AadharCardPath { get; set; }

    [StringLength(50)]
    [Display(Name = "MCA Card No.")]
    public string? MCACardNo { get; set; }

    public string? MCACardPath { get; set; }

    [Display(Name = "Joining Date")]
    [DataType(DataType.Date)]
    public DateTime JoiningDate { get; set; } = DateTime.Today;

    public int? BatchId { get; set; }
    public Batch? Batch { get; set; }

    [NotMapped]
    public int? Age => DateOfBirth.HasValue
        ? (int)((DateTime.Today - DateOfBirth.Value).TotalDays / 365.25)
        : null;

    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    public ICollection<FeePayment> FeePayments { get; set; } = new List<FeePayment>();
    public ICollection<FitnessTestResult> FitnessTestResults { get; set; } = new List<FitnessTestResult>();
    public ICollection<MovementScreeningResult> MovementScreeningResults { get; set; } = new List<MovementScreeningResult>();
    public ICollection<SkillAssessmentResult> SkillAssessmentResults { get; set; } = new List<SkillAssessmentResult>();
    public ICollection<SCAssessmentResult> SCAssessmentResults { get; set; } = new List<SCAssessmentResult>();
    public ICollection<MentalToughnessResult> MentalToughnessResults { get; set; } = new List<MentalToughnessResult>();
    public ICollection<TeamworkResult> TeamworkResults { get; set; } = new List<TeamworkResult>();
    public ICollection<InjuryRecord> InjuryRecords { get; set; } = new List<InjuryRecord>();
    public ICollection<NutritionLog> NutritionLogs { get; set; } = new List<NutritionLog>();
    public ICollection<NutritionGoal> NutritionGoals { get; set; } = new List<NutritionGoal>();
}
