using System.ComponentModel.DataAnnotations;

namespace SportsAcademy.Models;

public class SkillGroup
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
}

public class Skill
{
    public int Id { get; set; }

    [Required]
    public int SkillGroupId { get; set; }
    public SkillGroup? SkillGroup { get; set; }

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

    public ICollection<SkillAssessmentResult> Results { get; set; } = new List<SkillAssessmentResult>();
}

public class SkillAssessmentResult
{
    public int Id { get; set; }
    public int SkillId { get; set; }
    public Skill? Skill { get; set; }

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
