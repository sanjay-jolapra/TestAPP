namespace SportsAcademy.Models.Auth;

public class AppModule
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string? Icon { get; set; }
    public string? Section { get; set; }
    public int SortOrder { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}
