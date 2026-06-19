using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Models;

namespace SportsAcademy.Controllers;

public class StrengthConditioningController : Controller
{
    private readonly ApplicationDbContext _db;
    public StrengthConditioningController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
        => View(await _db.SCGroups.Include(g => g.Exercises).ToListAsync());

    public IActionResult CreateGroup() => View(new SCGroup());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateGroup(SCGroup group)
    {
        if (ModelState.IsValid) { _db.SCGroups.Add(group); await _db.SaveChangesAsync(); TempData["Success"] = "Group added!"; return RedirectToAction(nameof(Index)); }
        return View(group);
    }

    public async Task<IActionResult> CreateExercise(int? groupId)
    {
        ViewBag.Groups = new SelectList(await _db.SCGroups.ToListAsync(), "Id", "Name", groupId);
        return View(new SCExercise { SCGroupId = groupId ?? 0 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateExercise(SCExercise exercise)
    {
        if (ModelState.IsValid) { _db.SCExercises.Add(exercise); await _db.SaveChangesAsync(); TempData["Success"] = "Exercise added!"; return RedirectToAction(nameof(Index)); }
        ViewBag.Groups = new SelectList(await _db.SCGroups.ToListAsync(), "Id", "Name", exercise.SCGroupId);
        return View(exercise);
    }

    public async Task<IActionResult> RecordResult(int? exerciseId, int? studentId)
    {
        ViewBag.Exercises = new SelectList(await _db.SCExercises.Include(e => e.SCGroup).Where(e => e.IsActive).Select(e => new { e.Id, Name = e.SCGroup!.Name + " - " + e.Name }).ToListAsync(), "Id", "Name", exerciseId);
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", studentId);
        return View(new SCAssessmentResult { AssessmentDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordResult(SCAssessmentResult result)
    {
        if (ModelState.IsValid) { _db.SCAssessmentResults.Add(result); await _db.SaveChangesAsync(); TempData["Success"] = "Result recorded!"; return RedirectToAction(nameof(Results)); }
        ViewBag.Exercises = new SelectList(await _db.SCExercises.Include(e => e.SCGroup).Where(e => e.IsActive).Select(e => new { e.Id, Name = e.SCGroup!.Name + " - " + e.Name }).ToListAsync(), "Id", "Name", result.SCExerciseId);
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", result.StudentId);
        return View(result);
    }

    public async Task<IActionResult> Results(int? exerciseId, int? studentId)
    {
        ViewBag.Exercises = new SelectList(await _db.SCExercises.Include(e => e.SCGroup).Select(e => new { e.Id, Name = e.SCGroup!.Name + " - " + e.Name }).ToListAsync(), "Id", "Name", exerciseId);
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", studentId);
        var query = _db.SCAssessmentResults.Include(r => r.SCExercise).ThenInclude(e => e!.SCGroup).Include(r => r.Student).AsQueryable();
        if (exerciseId.HasValue) query = query.Where(r => r.SCExerciseId == exerciseId);
        if (studentId.HasValue) query = query.Where(r => r.StudentId == studentId);
        return View(await query.OrderByDescending(r => r.AssessmentDate).ToListAsync());
    }
}
