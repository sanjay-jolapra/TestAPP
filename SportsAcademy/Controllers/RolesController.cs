using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Infrastructure;
using SportsAcademy.Models;

namespace SportsAcademy.Controllers;

[RequirePermission(AppModules.UserManagement, "View")]
public class RolesController : Controller
{
    private readonly ApplicationDbContext _db;
    public RolesController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
        => View(await _db.Roles.Include(r => r.Users).Include(r => r.Permissions).ToListAsync());

    [RequirePermission(AppModules.UserManagement, "Create")]
    public IActionResult Create() => View(new Role());

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(AppModules.UserManagement, "Create")]
    public async Task<IActionResult> Create(Role role)
    {
        if (ModelState.IsValid)
        {
            _db.Roles.Add(role);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Role created! Now set its permissions.";
            return RedirectToAction(nameof(Permissions), new { id = role.Id });
        }
        return View(role);
    }

    [RequirePermission(AppModules.UserManagement, "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var role = await _db.Roles.FindAsync(id);
        if (role == null) return NotFound();
        return View(role);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(AppModules.UserManagement, "Edit")]
    public async Task<IActionResult> Edit(int id, Role role)
    {
        if (id != role.Id) return BadRequest();
        if (ModelState.IsValid)
        {
            _db.Roles.Update(role);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Role updated!";
            return RedirectToAction(nameof(Index));
        }
        return View(role);
    }

    [RequirePermission(AppModules.UserManagement, "Edit")]
    public async Task<IActionResult> Permissions(int id)
    {
        var role = await _db.Roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Id == id);
        if (role == null) return NotFound();

        // Ensure all modules have a permission row
        var existing = role.Permissions.ToDictionary(p => p.Module);
        var vm = AppModules.All.Select(module => existing.TryGetValue(module, out var p)
            ? p
            : new RolePermission { RoleId = id, Module = module }).ToList();

        ViewBag.Role = role;
        ViewBag.Labels = AppModules.Labels;
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(AppModules.UserManagement, "Edit")]
    public async Task<IActionResult> SavePermissions(int id, List<RolePermission> permissions)
    {
        var role = await _db.Roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Id == id);
        if (role == null) return NotFound();

        foreach (var incoming in permissions)
        {
            var existing = role.Permissions.FirstOrDefault(p => p.Module == incoming.Module);
            if (existing != null)
            {
                existing.CanView   = incoming.CanView;
                existing.CanCreate = incoming.CanCreate;
                existing.CanEdit   = incoming.CanEdit;
                existing.CanDelete = incoming.CanDelete;
            }
            else
            {
                _db.RolePermissions.Add(new RolePermission
                {
                    RoleId    = id,
                    Module    = incoming.Module,
                    CanView   = incoming.CanView,
                    CanCreate = incoming.CanCreate,
                    CanEdit   = incoming.CanEdit,
                    CanDelete = incoming.CanDelete
                });
            }
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = "Permissions saved!";
        return RedirectToAction(nameof(Permissions), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(AppModules.UserManagement, "Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var hasUsers = await _db.AppUsers.AnyAsync(u => u.RoleId == id);
        if (hasUsers)
        {
            TempData["Error"] = "Cannot delete a role that has users assigned.";
            return RedirectToAction(nameof(Index));
        }
        var role = await _db.Roles.FindAsync(id);
        if (role != null) { _db.Roles.Remove(role); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Role deleted.";
        return RedirectToAction(nameof(Index));
    }
}
