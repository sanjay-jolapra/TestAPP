using System.ComponentModel.DataAnnotations;

namespace SportsAcademy.Models.ViewModels;

public class RoleViewModel
{
    public int Id { get; set; }

    [Required, StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = "";

    [StringLength(200)]
    public string? Description { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public List<ModulePermissionViewModel> ModulePermissions { get; set; } = [];
}

public class ModulePermissionViewModel
{
    public int ModuleId { get; set; }
    public string ModuleName { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string? Icon { get; set; }
    public string? Section { get; set; }
    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
}
