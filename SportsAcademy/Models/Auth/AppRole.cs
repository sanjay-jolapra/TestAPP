namespace SportsAcademy.Models.Auth;

public class AppRole
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<AppUser> Users { get; set; } = [];
    public ICollection<RolePermission> Permissions { get; set; } = [];
}
