using SportsAcademy.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Models;

namespace SportsAcademy.Controllers;

[ModulePermission("FitnessTests")]
public class FitnessTestsController : Controller
{
    private readonly ApplicationDbContext _db;
    public FitnessTestsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var tests = await _db.FitnessTests.Include(t => t.Results).ToListAsync();
        return View(tests);
    }

    public IActionResult CreateTest() => View(new FitnessTest());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTest(FitnessTest test)
    {
        if (ModelState.IsValid)
        {
            _db.FitnessTests.Add(test);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Test added!";
            return RedirectToAction(nameof(Index));
        }
        return View(test);
    }

    public async Task<IActionResult> EditTest(int id)
    {
        var test = await _db.FitnessTests.FindAsync(id);
        if (test == null) return NotFound();
        return View(test);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTest(int id, FitnessTest test)
    {
        if (id != test.Id) return BadRequest();
        if (ModelState.IsValid)
        {
            _db.FitnessTests.Update(test);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Test updated!";
            return RedirectToAction(nameof(Index));
        }
        return View(test);
    }

    public async Task<IActionResult> RecordResult(int? testId, int? studentId)
    {
        ViewBag.Tests = new SelectList(await _db.FitnessTests.Where(t => t.IsActive).ToListAsync(), "Id", "Name", testId);
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", studentId);
        return View(new FitnessTestResult { TestDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordResult(FitnessTestResult result)
    {
        if (ModelState.IsValid)
        {
            _db.FitnessTestResults.Add(result);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Result recorded!";
            return RedirectToAction(nameof(Results));
        }
        ViewBag.Tests = new SelectList(await _db.FitnessTests.Where(t => t.IsActive).ToListAsync(), "Id", "Name", result.FitnessTestId);
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", result.StudentId);
        return View(result);
    }

    public async Task<IActionResult> Results(int? testId, int? studentId)
    {
        ViewBag.Tests = new SelectList(await _db.FitnessTests.ToListAsync(), "Id", "Name", testId);
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", studentId);

        var query = _db.FitnessTestResults.Include(r => r.FitnessTest).Include(r => r.Student).AsQueryable();
        if (testId.HasValue) query = query.Where(r => r.FitnessTestId == testId);
        if (studentId.HasValue) query = query.Where(r => r.StudentId == studentId);

        var results = await query.OrderByDescending(r => r.TestDate).ToListAsync();
        return View(results);
    }
}
