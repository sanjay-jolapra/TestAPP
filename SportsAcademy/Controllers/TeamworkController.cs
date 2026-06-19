using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Models;

namespace SportsAcademy.Controllers;

public class TeamworkController : Controller
{
    private readonly ApplicationDbContext _db;
    public TeamworkController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
        => View(await _db.TeamworkTests.Include(t => t.Results).ToListAsync());

    public IActionResult CreateTest() => View(new TeamworkTest());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTest(TeamworkTest test)
    {
        if (ModelState.IsValid) { _db.TeamworkTests.Add(test); await _db.SaveChangesAsync(); TempData["Success"] = "Test added!"; return RedirectToAction(nameof(Index)); }
        return View(test);
    }

    public async Task<IActionResult> EditTest(int id)
    {
        var t = await _db.TeamworkTests.FindAsync(id);
        if (t == null) return NotFound();
        return View(t);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTest(int id, TeamworkTest test)
    {
        if (id != test.Id) return BadRequest();
        if (ModelState.IsValid) { _db.TeamworkTests.Update(test); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        return View(test);
    }

    public async Task<IActionResult> RecordResult(int? testId, int? studentId)
    {
        ViewBag.Tests = new SelectList(await _db.TeamworkTests.Where(t => t.IsActive).ToListAsync(), "Id", "Name", testId);
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", studentId);
        return View(new TeamworkResult { AssessmentDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordResult(TeamworkResult result)
    {
        if (ModelState.IsValid) { _db.TeamworkResults.Add(result); await _db.SaveChangesAsync(); TempData["Success"] = "Result recorded!"; return RedirectToAction(nameof(Results)); }
        ViewBag.Tests = new SelectList(await _db.TeamworkTests.Where(t => t.IsActive).ToListAsync(), "Id", "Name", result.TeamworkTestId);
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", result.StudentId);
        return View(result);
    }

    public async Task<IActionResult> Results(int? testId, int? studentId)
    {
        ViewBag.Tests = new SelectList(await _db.TeamworkTests.ToListAsync(), "Id", "Name", testId);
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", studentId);
        var query = _db.TeamworkResults.Include(r => r.TeamworkTest).Include(r => r.Student).AsQueryable();
        if (testId.HasValue) query = query.Where(r => r.TeamworkTestId == testId);
        if (studentId.HasValue) query = query.Where(r => r.StudentId == studentId);
        return View(await query.OrderByDescending(r => r.AssessmentDate).ToListAsync());
    }
}
