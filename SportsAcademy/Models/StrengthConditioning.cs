using System.ComponentModel.DataAnnotations;

namespace SportsAcademy.Models;

public class SCGroup
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<SCExercise> Exercises { get; set; } = new List<SCExercise>();
}

public class SCExercise
{
    public int Id { get; set; }

    [Required]
    public int SCGroupId { get; set; }
    public SCGroup? SCGroup { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = "";

    public string? Description { get; set; }

    [StringLength(100)]
    [Display(Name = "Measurement Unit")]
    public string? MeasurementUnit { get; set; }

    [StringLength(150)]
    [Display(Name = "Measures")]
    public string? Measures { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<SCAssessmentResult> Results { get; set; } = new List<SCAssessmentResult>();
}

public class SCAssessmentResult
{
    public int Id { get; set; }
    public int SCExerciseId { get; set; }
    public SCExercise? SCExercise { get; set; }

    public int StudentId { get; set; }
    public Student? Student { get; set; }

    [Required, DataType(DataType.Date)]
    [Display(Name = "Assessment Date")]
    public DateTime AssessmentDate { get; set; }

    [Required, StringLength(100)]
    public string Value { get; set; } = "";

    [Range(1, 10)]
    public int? Score { get; set; }

    public string? Notes { get; set; }
}
