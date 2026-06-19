using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Filters;
using SportsAcademy.Models.Auth;
using SportsAcademy.Models.ViewModels;
using SportsAcademy.Services;

namespace SportsAcademy.Controllers;

[ModulePermission("RoleManagement")]
public class RoleManagementController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IPermissionService _permissionService;

    public RoleManagementController(ApplicationDbContext db, IPermissionService permissionService)
    {
        _db = db;
        _permissionService = permissionService;
    }

    public async Task<IActionResult> Index()
    {
        var roles = await _db.AppRoles
            .Include(r => r.Users)
            .Include(r => r.Permissions)
            .OrderBy(r => r.Name)
            .ToListAsync();
        return View(roles);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var modules = await _db.AppModules.OrderBy(m => m.SortOrder).ToListAsync();
        var vm = new RoleViewModel
        {
            IsActive = true,
            ModulePermissions = modules.Select(m => new ModulePermissionViewModel
            {
                ModuleId = m.Id,
                ModuleName = m.Name,
                DisplayName = m.DisplayName,
                Icon = m.Icon,
                Section = m.Section
            }).ToList()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoleViewModel model)
    {
        if (await _db.AppRoles.AnyAsync(r => r.Name == model.Name))
            ModelState.AddModelError("Name", "A role with this name already exists.");

        if (!ModelState.IsValid)
        {
            // Re-populate module list to keep UI consistent
            var modules = await _db.AppModules.OrderBy(m => m.SortOrder).ToListAsync();
            foreach (var m in modules.Where(m => !model.ModulePermissions.Any(mp => mp.ModuleId == m.Id)))
                model.ModulePermissions.Add(new ModulePermissionViewModel
                {
                    ModuleId = m.Id, ModuleName = m.Name, DisplayName = m.DisplayName,
                    Icon = m.Icon, Section = m.Section
                });
            return View(model);
        }

        var role = new AppRole
        {
            Name = model.Name.Trim(),
            Description = model.Description?.Trim(),
            IsActive = model.IsActive,
            CreatedAt = DateTime.UtcNow
        };
        _db.AppRoles.Add(role);
        await _db.SaveChangesAsync();

        await SavePermissionsAsync(role.Id, model.ModulePermissions);

        TempData["Success"] = $"Role '{role.Name}' created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var role = await _db.AppRoles.Include(r => r.Permissions).ThenInclude(p => p.Module)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (role == null) return NotFound();

        var modules = await _db.AppModules.OrderBy(m => m.SortOrder).ToListAsync();
        var vm = new RoleViewModel
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive,
            ModulePermissions = modules.Select(m =>
            {
                var existing = role.Permissions.FirstOrDefault(p => p.ModuleId == m.Id);
                return new ModulePermissionViewModel
                {
                    ModuleId = m.Id,
                    ModuleName = m.Name,
                    DisplayName = m.DisplayName,
                    Icon = m.Icon,
                    Section = m.Section,
                    CanView = existing?.CanView ?? false,
                    CanCreate = existing?.CanCreate ?? false,
                    CanEdit = existing?.CanEdit ?? false,
                    CanDelete = existing?.CanDelete ?? false,
                };
            }).ToList()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RoleViewModel model)
    {
        if (await _db.AppRoles.AnyAsync(r => r.Name == model.Name && r.Id != id))
            ModelState.AddModelError("Name", "A role with this name already exists.");

        if (!ModelState.IsValid)
        {
            var modules = await _db.AppModules.OrderBy(m => m.SortOrder).ToListAsync();
            foreach (var m in modules.Where(m => !model.ModulePermissions.Any(mp => mp.ModuleId == m.Id)))
                model.ModulePermissions.Add(new ModulePermissionViewModel
                {
                    ModuleId = m.Id, ModuleName = m.Name, DisplayName = m.DisplayName,
                    Icon = m.Icon, Section = m.Section
                });
            return View(model);
        }

        var role = await _db.AppRoles.FindAsync(id);
        if (role == null) return NotFound();

        // Protect SuperAdmin role name
        if (role.Name == "SuperAdmin") model.Name = "SuperAdmin";

        role.Name = model.Name.Trim();
        role.Description = model.Description?.Trim();
        role.IsActive = model.IsActive;
        await _db.SaveChangesAsync();

        // Remove existing permissions and re-save
        var old = _db.RolePermissions.Where(rp => rp.RoleId == id);
        _db.RolePermissions.RemoveRange(old);
        await _db.SaveChangesAsync();

        await SavePermissionsAsync(id, model.ModulePermissions);
        await _permissionService.InvalidateCacheAsync(id);

        TempData["Success"] = $"Role '{role.Name}' updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var role = await _db.AppRoles.Include(r => r.Users).FirstOrDefaultAsync(r => r.Id == id);
        if (role == null) return NotFound();

        if (role.Name == "SuperAdmin")
        {
            TempData["Error"] = "The SuperAdmin role cannot be deleted.";
            return RedirectToAction(nameof(Index));
        }

        if (role.Users.Any())
        {
            TempData["Error"] = $"Role '{role.Name}' has {role.Users.Count} assigned user(s). Reassign users before deleting.";
            return RedirectToAction(nameof(Index));
        }

        _db.RolePermissions.RemoveRange(_db.RolePermissions.Where(rp => rp.RoleId == id));
        _db.AppRoles.Remove(role);
        await _db.SaveChangesAsync();
        await _permissionService.InvalidateCacheAsync(id);

        TempData["Success"] = $"Role '{role.Name}' deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task SavePermissionsAsync(int roleId, List<ModulePermissionViewModel> perms)
    {
        var entities = perms
            .Where(p => p.CanView || p.CanCreate || p.CanEdit || p.CanDelete)
            .Select(p => new RolePermission
            {
                RoleId = roleId,
                ModuleId = p.ModuleId,
                CanView = p.CanView,
                CanCreate = p.CanCreate,
                CanEdit = p.CanEdit,
                CanDelete = p.CanDelete
            });
        _db.RolePermissions.AddRange(entities);
        await _db.SaveChangesAsync();
    }
}
