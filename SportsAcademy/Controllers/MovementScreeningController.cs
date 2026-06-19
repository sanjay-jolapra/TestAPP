using SportsAcademy.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Models;

namespace SportsAcademy.Controllers;

[ModulePermission("MovementScreening")]
public class MovementScreeningController : Controller
{
    private readonly ApplicationDbContext _db;
    public MovementScreeningController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
        => View(await _db.MovementScreeningTests.Include(t => t.Results).ToListAsync());

    public IActionResult CreateTest() => View(new MovementScreeningTest());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTest(MovementScreeningTest test)
    {
        if (ModelState.IsValid) { _db.MovementScreeningTests.Add(test); await _db.SaveChangesAsync(); TempData["Success"] = "Test added!"; return RedirectToAction(nameof(Index)); }
        return View(test);
    }

    public async Task<IActionResult> EditTest(int id)
    {
        var t = await _db.MovementScreeningTests.FindAsync(id);
        if (t == null) return NotFound();
        return View(t);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTest(int id, MovementScreeningTest test)
    {
        if (id != test.Id) return BadRequest();
        if (ModelState.IsValid) { _db.MovementScreeningTests.Update(test); await _db.SaveChangesAsync(); TempData["Success"] = "Updated!"; return RedirectToAction(nameof(Index)); }
        return View(test);
    }

    public async Task<IActionResult> RecordResult(int? testId, int? studentId)
    {
        ViewBag.Tests = new SelectList(await _db.MovementScreeningTests.Where(t => t.IsActive).ToListAsync(), "Id", "Name", testId);
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", studentId);
        return View(new MovementScreeningResult { TestDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordResult(MovementScreeningResult result)
    {
        if (ModelState.IsValid) { _db.MovementScreeningResults.Add(result); await _db.SaveChangesAsync(); TempData["Success"] = "Result recorded!"; return RedirectToAction(nameof(Results)); }
        ViewBag.Tests = new SelectList(await _db.MovementScreeningTests.Where(t => t.IsActive).ToListAsync(), "Id", "Name", result.MovementScreeningTestId);
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", result.StudentId);
        return View(result);
    }

    public async Task<IActionResult> Results(int? testId, int? studentId)
    {
        ViewBag.Tests = new SelectList(await _db.MovementScreeningTests.ToListAsync(), "Id", "Name", testId);
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", studentId);
        var query = _db.MovementScreeningResults.Include(r => r.MovementScreeningTest).Include(r => r.Student).AsQueryable();
        if (testId.HasValue) query = query.Where(r => r.MovementScreeningTestId == testId);
        if (studentId.HasValue) query = query.Where(r => r.StudentId == studentId);
        return View(await query.OrderByDescending(r => r.TestDate).ToListAsync());
    }
}
