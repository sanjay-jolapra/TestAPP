using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Infrastructure;
using SportsAcademy.Models;

namespace SportsAcademy.Controllers;

[RequirePermission(AppModules.UserManagement, "View")]
public class UsersController : Controller
{
    private readonly ApplicationDbContext _db;
    public UsersController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
        => View(await _db.AppUsers.Include(u => u.Role).OrderBy(u => u.FullName).ToListAsync());

    [RequirePermission(AppModules.UserManagement, "Create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = new SelectList(await _db.Roles.Where(r => r.IsActive).ToListAsync(), "Id", "Name");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(AppModules.UserManagement, "Create")]
    public async Task<IActionResult> Create(CreateUserVM model)
    {
        if (await _db.AppUsers.AnyAsync(u => u.Username == model.Username))
            ModelState.AddModelError("Username", "Username already taken.");

        if (!ModelState.IsValid)
        {
            ViewBag.Roles = new SelectList(await _db.Roles.Where(r => r.IsActive).ToListAsync(), "Id", "Name");
            return View(model);
        }

        _db.AppUsers.Add(new AppUser
        {
            FullName = model.FullName,
            Username = model.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password, workFactor: 12),
            Email = model.Email,
            Phone = model.Phone,
            RoleId = model.RoleId,
            IsActive = model.IsActive
        });
        await _db.SaveChangesAsync();
        TempData["Success"] = "User created!";
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission(AppModules.UserManagement, "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _db.AppUsers.FindAsync(id);
        if (user == null) return NotFound();
        ViewBag.Roles = new SelectList(await _db.Roles.Where(r => r.IsActive).ToListAsync(), "Id", "Name", user.RoleId);
        return View(new EditUserVM
        {
            Id = user.Id, FullName = user.FullName, Username = user.Username,
            Email = user.Email, Phone = user.Phone, RoleId = user.RoleId, IsActive = user.IsActive
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(AppModules.UserManagement, "Edit")]
    public async Task<IActionResult> Edit(int id, EditUserVM model)
    {
        if (id != model.Id) return BadRequest();

        if (await _db.AppUsers.AnyAsync(u => u.Username == model.Username && u.Id != id))
            ModelState.AddModelError("Username", "Username already taken.");

        if (!ModelState.IsValid)
        {
            ViewBag.Roles = new SelectList(await _db.Roles.Where(r => r.IsActive).ToListAsync(), "Id", "Name", model.RoleId);
            return View(model);
        }

        var user = await _db.AppUsers.FindAsync(id);
        if (user == null) return NotFound();

        user.FullName = model.FullName;
        user.Username = model.Username;
        user.Email = model.Email;
        user.Phone = model.Phone;
        user.RoleId = model.RoleId;
        user.IsActive = model.IsActive;

        if (!string.IsNullOrWhiteSpace(model.NewPassword))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword, workFactor: 12);

        await _db.SaveChangesAsync();
        TempData["Success"] = "User updated!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(AppModules.UserManagement, "Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        if (id == currentUserId)
        {
            TempData["Error"] = "You cannot delete your own account.";
            return RedirectToAction(nameof(Index));
        }
        var user = await _db.AppUsers.FindAsync(id);
        if (user != null) { _db.AppUsers.Remove(user); await _db.SaveChangesAsync(); }
        TempData["Success"] = "User deleted.";
        return RedirectToAction(nameof(Index));
    }
}

public class CreateUserVM
{
    [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.StringLength(100)]
    [System.ComponentModel.DataAnnotations.Display(Name = "Full Name")]
    public string FullName { get; set; } = "";

    [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.StringLength(100)]
    public string Username { get; set; } = "";

    [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.MinLength(6)]
    [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
    public string Password { get; set; } = "";

    [System.ComponentModel.DataAnnotations.EmailAddress]
    public string? Email { get; set; }
    public string? Phone { get; set; }

    [System.ComponentModel.DataAnnotations.Required]
    public int RoleId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class EditUserVM
{
    public int Id { get; set; }

    [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.StringLength(100)]
    [System.ComponentModel.DataAnnotations.Display(Name = "Full Name")]
    public string FullName { get; set; } = "";

    [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.StringLength(100)]
    public string Username { get; set; } = "";

    [System.ComponentModel.DataAnnotations.MinLength(6)]
    [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
    [System.ComponentModel.DataAnnotations.Display(Name = "New Password")]
    public string? NewPassword { get; set; }

    [System.ComponentModel.DataAnnotations.EmailAddress]
    public string? Email { get; set; }
    public string? Phone { get; set; }

    [System.ComponentModel.DataAnnotations.Required]
    public int RoleId { get; set; }
    public bool IsActive { get; set; } = true;
}
