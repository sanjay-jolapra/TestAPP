using SportsAcademy.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Models;

namespace SportsAcademy.Controllers;

[ModulePermission("SkillDevelopment")]
public class SkillDevelopmentController : Controller
{
    private readonly ApplicationDbContext _db;
    public SkillDevelopmentController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
        => View(await _db.SkillGroups.Include(g => g.Skills).ToListAsync());

    public IActionResult CreateGroup() => View(new SkillGroup());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateGroup(SkillGroup group)
    {
        if (ModelState.IsValid) { _db.SkillGroups.Add(group); await _db.SaveChangesAsync(); TempData["Success"] = "Group added!"; return RedirectToAction(nameof(Index)); }
        return View(group);
    }

    public async Task<IActionResult> CreateSkill(int? groupId)
    {
        ViewBag.Groups = new SelectList(await _db.SkillGroups.ToListAsync(), "Id", "Name", groupId);
        return View(new Skill { SkillGroupId = groupId ?? 0 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSkill(Skill skill)
    {
        if (ModelState.IsValid) { _db.Skills.Add(skill); await _db.SaveChangesAsync(); TempData["Success"] = "Skill added!"; return RedirectToAction(nameof(Index)); }
        ViewBag.Groups = new SelectList(await _db.SkillGroups.ToListAsync(), "Id", "Name", skill.SkillGroupId);
        return View(skill);
    }

    public async Task<IActionResult> RecordResult(int? skillId, int? studentId)
    {
        ViewBag.Skills = new SelectList(await _db.Skills.Include(s => s.SkillGroup).Where(s => s.IsActive).Select(s => new { s.Id, Name = s.SkillGroup!.Name + " - " + s.Name }).ToListAsync(), "Id", "Name", skillId);
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", studentId);
        return View(new SkillAssessmentResult { AssessmentDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordResult(SkillAssessmentResult result)
    {
        if (ModelState.IsValid) { _db.SkillAssessmentResults.Add(result); await _db.SaveChangesAsync(); TempData["Success"] = "Result recorded!"; return RedirectToAction(nameof(Results)); }
        ViewBag.Skills = new SelectList(await _db.Skills.Include(s => s.SkillGroup).Where(s => s.IsActive).Select(s => new { s.Id, Name = s.SkillGroup!.Name + " - " + s.Name }).ToListAsync(), "Id", "Name", result.SkillId);
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", result.StudentId);
        return View(result);
    }

    public async Task<IActionResult> Results(int? skillId, int? studentId)
    {
        ViewBag.Skills = new SelectList(await _db.Skills.Include(s => s.SkillGroup).Select(s => new { s.Id, Name = s.SkillGroup!.Name + " - " + s.Name }).ToListAsync(), "Id", "Name", skillId);
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", studentId);
        var query = _db.SkillAssessmentResults.Include(r => r.Skill).ThenInclude(s => s!.SkillGroup).Include(r => r.Student).AsQueryable();
        if (skillId.HasValue) query = query.Where(r => r.SkillId == skillId);
        if (studentId.HasValue) query = query.Where(r => r.StudentId == studentId);
        return View(await query.OrderByDescending(r => r.AssessmentDate).ToListAsync());
    }
}
