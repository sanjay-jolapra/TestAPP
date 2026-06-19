using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Models;

namespace SportsAcademy.Controllers;

public class BatchesController : Controller
{
    private readonly ApplicationDbContext _db;
    public BatchesController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
        => View(await _db.Batches.Include(b => b.Students).ToListAsync());

    public IActionResult Create() => View(new Batch());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Batch batch)
    {
        if (ModelState.IsValid) { _db.Batches.Add(batch); await _db.SaveChangesAsync(); TempData["Success"] = "Batch added!"; return RedirectToAction(nameof(Index)); }
        return View(batch);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var b = await _db.Batches.FindAsync(id);
        if (b == null) return NotFound();
        return View(b);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Batch batch)
    {
        if (id != batch.Id) return BadRequest();
        if (ModelState.IsValid) { _db.Batches.Update(batch); await _db.SaveChangesAsync(); TempData["Success"] = "Updated!"; return RedirectToAction(nameof(Index)); }
        return View(batch);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var b = await _db.Batches.FindAsync(id);
        if (b != null) { _db.Batches.Remove(b); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
