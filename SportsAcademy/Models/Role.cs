using System.ComponentModel.DataAnnotations;

namespace SportsAcademy.Models;

public class Role
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    [StringLength(300)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
    public ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();
}
