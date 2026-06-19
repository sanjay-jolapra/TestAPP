using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Filters;
using SportsAcademy.Models.Auth;
using SportsAcademy.Models.ViewModels;
using SportsAcademy.Services;

namespace SportsAcademy.Controllers;

[ModulePermission("UserManagement")]
public class UserManagementController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IAuthService _authService;

    public UserManagementController(ApplicationDbContext db, IAuthService authService)
    {
        _db = db;
        _authService = authService;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _db.AppUsers
            .Include(u => u.Role)
            .OrderBy(u => u.Username)
            .ToListAsync();
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = new SelectList(
            await _db.AppRoles.Where(r => r.IsActive).OrderBy(r => r.Name).ToListAsync(),
            "Id", "Name");
        return View(new UserViewModel { IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserViewModel model)
    {
        if (string.IsNullOrEmpty(model.Password))
            ModelState.AddModelError("Password", "Password is required for new users.");

        if (await _db.AppUsers.AnyAsync(u => u.Username == model.Username))
            ModelState.AddModelError("Username", "Username already exists.");

        if (!ModelState.IsValid)
        {
            ViewBag.Roles = new SelectList(
                await _db.AppRoles.Where(r => r.IsActive).OrderBy(r => r.Name).ToListAsync(),
                "Id", "Name");
            return View(model);
        }

        var salt = _authService.GenerateSalt();
        var user = new AppUser
        {
            Username = model.Username.Trim(),
            Email = model.Email.Trim(),
            FullName = model.FullName.Trim(),
            PasswordSalt = salt,
            PasswordHash = _authService.HashPassword(model.Password!, salt),
            RoleId = model.RoleId,
            IsActive = model.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _db.AppUsers.Add(user);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"User '{user.Username}' created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _db.AppUsers.FindAsync(id);
        if (user == null) return NotFound();

        ViewBag.Roles = new SelectList(
            await _db.AppRoles.Where(r => r.IsActive).OrderBy(r => r.Name).ToListAsync(),
            "Id", "Name", user.RoleId);

        return View(new UserViewModel
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            RoleId = user.RoleId,
            IsActive = user.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserViewModel model)
    {
        if (await _db.AppUsers.AnyAsync(u => u.Username == model.Username && u.Id != id))
            ModelState.AddModelError("Username", "Username already exists.");

        if (!ModelState.IsValid)
        {
            ViewBag.Roles = new SelectList(
                await _db.AppRoles.Where(r => r.IsActive).OrderBy(r => r.Name).ToListAsync(),
                "Id", "Name");
            return View(model);
        }

        var user = await _db.AppUsers.FindAsync(id);
        if (user == null) return NotFound();

        user.Email = model.Email.Trim();
        user.FullName = model.FullName.Trim();
        user.Username = model.Username.Trim();
        user.RoleId = model.RoleId;
        user.IsActive = model.IsActive;

        if (!string.IsNullOrEmpty(model.Password))
        {
            user.PasswordSalt = _authService.GenerateSalt();
            user.PasswordHash = _authService.HashPassword(model.Password, user.PasswordSalt);
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = $"User '{user.Username}' updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.AppUsers.FindAsync(id);
        if (user == null) return NotFound();

        if (user.Username == "admin")
        {
            TempData["Error"] = "The built-in admin account cannot be deleted.";
            return RedirectToAction(nameof(Index));
        }

        _db.AppUsers.Remove(user);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"User '{user.Username}' deleted.";
        return RedirectToAction(nameof(Index));
    }
}
