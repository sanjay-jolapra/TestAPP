using System.ComponentModel.DataAnnotations;

namespace SportsAcademy.Models;

public class AppUser
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string FullName { get; set; } = "";

    [Required, StringLength(100)]
    public string Username { get; set; } = "";

    [Required, StringLength(200)]
    public string PasswordHash { get; set; } = "";

    [StringLength(150), EmailAddress]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }

    public int RoleId { get; set; }
    public Role? Role { get; set; }
}
