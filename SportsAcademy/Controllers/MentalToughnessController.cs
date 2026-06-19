using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Models;

namespace SportsAcademy.Controllers;

public class MentalToughnessController : Controller
{
    private readonly ApplicationDbContext _db;
    public MentalToughnessController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
        => View(await _db.MentalToughnessTests.Include(t => t.Results).ToListAsync());

    public IActionResult CreateTest() => View(new MentalToughnessTest());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTest(MentalToughnessTest test)
    {
        if (ModelState.IsValid) { _db.MentalToughnessTests.Add(test); await _db.SaveChangesAsync(); TempData["Success"] = "Test added!"; return RedirectToAction(nameof(Index)); }
        return View(test);
    }

    public async Task<IActionResult> EditTest(int id)
    {
        var t = await _db.MentalToughnessTests.FindAsync(id);
        if (t == null) return NotFound();
        return View(t);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTest(int id, MentalToughnessTest test)
    {
        if (id != test.Id) return BadRequest();
        if (ModelState.IsValid) { _db.MentalToughnessTests.Update(test); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        return View(test);
    }

    public async Task<IActionResult> RecordResult(int? testId, int? studentId)
    {
        ViewBag.Tests = new SelectList(await _db.MentalToughnessTests.Where(t => t.IsActive).ToListAsync(), "Id", "Name", testId);
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", studentId);
        return View(new MentalToughnessResult { AssessmentDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordResult(MentalToughnessResult result)
    {
        if (ModelState.IsValid) { _db.MentalToughnessResults.Add(result); await _db.SaveChangesAsync(); TempData["Success"] = "Result recorded!"; return RedirectToAction(nameof(Results)); }
        ViewBag.Tests = new SelectList(await _db.MentalToughnessTests.Where(t => t.IsActive).ToListAsync(), "Id", "Name", result.MentalToughnessTestId);
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", result.StudentId);
        return View(result);
    }

    public async Task<IActionResult> Results(int? testId, int? studentId)
    {
        ViewBag.Tests = new SelectList(await _db.MentalToughnessTests.ToListAsync(), "Id", "Name", testId);
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", studentId);
        var query = _db.MentalToughnessResults.Include(r => r.MentalToughnessTest).Include(r => r.Student).AsQueryable();
        if (testId.HasValue) query = query.Where(r => r.MentalToughnessTestId == testId);
        if (studentId.HasValue) query = query.Where(r => r.StudentId == studentId);
        return View(await query.OrderByDescending(r => r.AssessmentDate).ToListAsync());
    }
}
