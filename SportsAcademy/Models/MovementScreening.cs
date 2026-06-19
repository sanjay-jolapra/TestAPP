using System.ComponentModel.DataAnnotations;

namespace SportsAcademy.Models;

public class MovementScreeningTest
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
    [Display(Name = "Measures")]
    public string? Measures { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<MovementScreeningResult> Results { get; set; } = new List<MovementScreeningResult>();
}

public class MovementScreeningResult
{
    public int Id { get; set; }
    public int MovementScreeningTestId { get; set; }
    public MovementScreeningTest? MovementScreeningTest { get; set; }

    public int StudentId { get; set; }
    public Student? Student { get; set; }

    [Required, DataType(DataType.Date)]
    [Display(Name = "Test Date")]
    public DateTime TestDate { get; set; }

    [Required, StringLength(100)]
    public string Value { get; set; } = "";

    [Range(1, 10)]
    public int? Score { get; set; }

    public string? Notes { get; set; }
}
