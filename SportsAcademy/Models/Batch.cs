using System.ComponentModel.DataAnnotations;

namespace SportsAcademy.Models;

public class Batch
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    public string? Description { get; set; }

    [Display(Name = "Start Time")]
    public TimeSpan? StartTime { get; set; }

    [Display(Name = "End Time")]
    public TimeSpan? EndTime { get; set; }

    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
}
