using System.ComponentModel.DataAnnotations;

namespace SportsAcademy.Models.ViewModels;

public class UserViewModel
{
    public int Id { get; set; }

    [Required, StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, StringLength(100, MinimumLength = 2), Display(Name = "Full Name")]
    public string FullName { get; set; } = "";

    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string? Password { get; set; }

    [Required, Display(Name = "Role")]
    public int RoleId { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
