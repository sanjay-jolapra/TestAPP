using System.ComponentModel.DataAnnotations;

namespace SportsAcademy.Models;

public enum AttendanceStatus { Present, Absent, Late }

public class AttendanceRecord
{
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Required]
    public AttendanceStatus Status { get; set; }

    public int? BatchId { get; set; }
    public Batch? Batch { get; set; }

    public string? Notes { get; set; }
}
