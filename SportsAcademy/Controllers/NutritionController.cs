using SportsAcademy.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Models;

namespace SportsAcademy.Controllers;

[ModulePermission("Nutrition")]
public class NutritionController : Controller
{
    private readonly ApplicationDbContext _db;
    public NutritionController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(int? studentId, DateTime? date)
    {
        date ??= DateTime.Today;
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", studentId);
        ViewBag.Date = date;

        if (!studentId.HasValue) return View(new List<NutritionLog>());

        var logs = await _db.NutritionLogs
            .Where(n => n.StudentId == studentId && n.Date.Date == date.Value.Date)
            .OrderBy(n => n.MealType)
            .ToListAsync();

        var goal = await _db.NutritionGoals.FirstOrDefaultAsync(g => g.StudentId == studentId && g.EffectiveFrom <= date.Value);
        ViewBag.Goal = goal;
        ViewBag.Student = await _db.Students.FindAsync(studentId);
        ViewBag.TotalCalories = logs.Sum(l => l.Calories ?? 0);
        ViewBag.TotalProtein = logs.Sum(l => l.Protein ?? 0);
        ViewBag.TotalCarbs = logs.Sum(l => l.Carbs ?? 0);
        ViewBag.TotalFats = logs.Sum(l => l.Fats ?? 0);
        ViewBag.TotalWater = logs.Sum(l => l.WaterMl ?? 0);
        return View(logs);
    }

    public async Task<IActionResult> LogMeal(int? studentId)
    {
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", studentId);
        return View(new NutritionLog { Date = DateTime.Today, StudentId = studentId ?? 0 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> LogMeal(NutritionLog log)
    {
        if (ModelState.IsValid) { _db.NutritionLogs.Add(log); await _db.SaveChangesAsync(); TempData["Success"] = "Meal logged!"; return RedirectToAction(nameof(Index), new { studentId = log.StudentId, date = log.Date.ToString("yyyy-MM-dd") }); }
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", log.StudentId);
        return View(log);
    }

    public async Task<IActionResult> SetGoal(int studentId)
    {
        var student = await _db.Students.FindAsync(studentId);
        if (student == null) return NotFound();
        ViewBag.Student = student;
        var existing = await _db.NutritionGoals.FirstOrDefaultAsync(g => g.StudentId == studentId);
        return View(existing ?? new NutritionGoal { StudentId = studentId, EffectiveFrom = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetGoal(NutritionGoal goal)
    {
        if (ModelState.IsValid)
        {
            var existing = await _db.NutritionGoals.FirstOrDefaultAsync(g => g.StudentId == goal.StudentId);
            if (existing != null) { existing.DailyCalories = goal.DailyCalories; existing.ProteinGoal = goal.ProteinGoal; existing.CarbsGoal = goal.CarbsGoal; existing.FatsGoal = goal.FatsGoal; existing.WaterGoal = goal.WaterGoal; existing.Notes = goal.Notes; }
            else _db.NutritionGoals.Add(goal);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Goals saved!";
            return RedirectToAction(nameof(Index), new { studentId = goal.StudentId });
        }
        ViewBag.Student = await _db.Students.FindAsync(goal.StudentId);
        return View(goal);
    }

    public async Task<IActionResult> Weekly(int studentId)
    {
        var student = await _db.Students.FindAsync(studentId);
        if (student == null) return NotFound();
        ViewBag.Student = student;
        var start = DateTime.Today.AddDays(-6);
        var logs = await _db.NutritionLogs
            .Where(n => n.StudentId == studentId && n.Date >= start)
            .OrderBy(n => n.Date)
            .ToListAsync();
        return View(logs);
    }
}
