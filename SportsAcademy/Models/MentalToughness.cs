using System.ComponentModel.DataAnnotations;

namespace SportsAcademy.Models;

public class MentalToughnessTest
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = "";

    public string? Description { get; set; }

    [StringLength(100)]
    [Display(Name = "Measurement Unit")]
    public string? MeasurementUnit { get; set; }

    [StringLength(150)]
    [Display(Name = "Measures")]
    public string? Measures { get; set; }

    [Range(1, 100)]
    [Display(Name = "Max Score")]
    public int MaxScore { get; set; } = 10;

    public bool IsActive { get; set; } = true;

    public ICollection<MentalToughnessResult> Results { get; set; } = new List<MentalToughnessResult>();
}

public class MentalToughnessResult
{
    public int Id { get; set; }
    public int MentalToughnessTestId { get; set; }
    public MentalToughnessTest? MentalToughnessTest { get; set; }

    public int StudentId { get; set; }
    public Student? Student { get; set; }

    [Required, DataType(DataType.Date)]
    [Display(Name = "Assessment Date")]
    public DateTime AssessmentDate { get; set; }

    [Range(0, 100)]
    public int Score { get; set; }

    public string? Notes { get; set; }
}
