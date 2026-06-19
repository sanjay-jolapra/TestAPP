using System.ComponentModel.DataAnnotations;

namespace SportsAcademy.Models;

public enum FeePeriodType { Monthly, Quarterly, Yearly }

public class FeePayment
{
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    [Required, DataType(DataType.Currency)]
    public decimal Amount { get; set; }

    [Required, DataType(DataType.Date)]
    [Display(Name = "Payment Date")]
    public DateTime PaidDate { get; set; }

    [Required]
    [Display(Name = "Period Type")]
    public FeePeriodType PeriodType { get; set; }

    [Display(Name = "Period Month (1-12)")]
    [Range(1, 12)]
    public int? PeriodMonth { get; set; }

    [Display(Name = "Period Year")]
    public int? PeriodYear { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [StringLength(100)]
    [Display(Name = "Receipt No.")]
    public string? ReceiptNo { get; set; }
}
