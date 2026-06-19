using System.ComponentModel.DataAnnotations;

namespace SportsAcademy.Models;

public enum InjurySeverity { Mild, Moderate, Severe }
public enum InjuryStatus { Active, Recovered, UnderTreatment }

public class InjuryRecord
{
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    [Required, DataType(DataType.Date)]
    [Display(Name = "Injury Date")]
    public DateTime InjuryDate { get; set; }

    [Required, StringLength(150)]
    [Display(Name = "Injury Type")]
    public string InjuryType { get; set; } = "";

    [StringLength(100)]
    [Display(Name = "Body Part")]
    public string? BodyPart { get; set; }

    [Required]
    public InjurySeverity Severity { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(1000)]
    [Display(Name = "Treatment Given")]
    public string? TreatmentGiven { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Expected Return Date")]
    public DateTime? ReturnDate { get; set; }

    [Required]
    public InjuryStatus Status { get; set; } = InjuryStatus.Active;

    [StringLength(200)]
    [Display(Name = "Doctor / Physio")]
    public string? DoctorPhysio { get; set; }
}
