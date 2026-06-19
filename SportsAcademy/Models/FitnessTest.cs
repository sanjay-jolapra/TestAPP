using System.ComponentModel.DataAnnotations;

namespace SportsAcademy.Models;

public class FitnessTest
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = "";

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(100)]
    [Display(Name = "Measurement Unit")]
    public string? MeasurementUnit { get; set; }

    [StringLength(150)]
    [Display(Name = "Measures (e.g. Speed, Agility)")]
    public string? Measures { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<FitnessTestResult> Results { get; set; } = new List<FitnessTestResult>();
}

public class FitnessTestResult
{
    public int Id { get; set; }
    public int FitnessTestId { get; set; }
    public FitnessTest? FitnessTest { get; set; }

    public int StudentId { get; set; }
    public Student? Student { get; set; }

    [Required, DataType(DataType.Date)]
    [Display(Name = "Test Date")]
    public DateTime TestDate { get; set; }

    [Required, StringLength(100)]
    public string Value { get; set; } = "";

    public string? Notes { get; set; }
}
