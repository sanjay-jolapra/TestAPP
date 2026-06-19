using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Models;

namespace SportsAcademy.Controllers;

public class InjuryTrackingController : Controller
{
    private readonly ApplicationDbContext _db;
    public InjuryTrackingController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(InjuryStatus? status)
    {
        var query = _db.InjuryRecords.Include(i => i.Student).AsQueryable();
        if (status.HasValue) query = query.Where(i => i.Status == status);
        ViewBag.Filter = status;
        return View(await query.OrderByDescending(i => i.InjuryDate).ToListAsync());
    }

    public async Task<IActionResult> Create(int? studentId)
    {
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", studentId);
        return View(new InjuryRecord { InjuryDate = DateTime.Today, Status = InjuryStatus.Active });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InjuryRecord record)
    {
        if (ModelState.IsValid) { _db.InjuryRecords.Add(record); await _db.SaveChangesAsync(); TempData["Success"] = "Injury recorded!"; return RedirectToAction(nameof(Index)); }
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", record.StudentId);
        return View(record);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var record = await _db.InjuryRecords.FindAsync(id);
        if (record == null) return NotFound();
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", record.StudentId);
        return View(record);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, InjuryRecord record)
    {
        if (id != record.Id) return BadRequest();
        if (ModelState.IsValid) { _db.InjuryRecords.Update(record); await _db.SaveChangesAsync(); TempData["Success"] = "Updated!"; return RedirectToAction(nameof(Index)); }
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", record.StudentId);
        return View(record);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var record = await _db.InjuryRecords.FindAsync(id);
        if (record != null) { _db.InjuryRecords.Remove(record); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Deleted.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> StudentHistory(int studentId)
    {
        var student = await _db.Students.FindAsync(studentId);
        if (student == null) return NotFound();
        ViewBag.Student = student;
        return View(await _db.InjuryRecords.Where(i => i.StudentId == studentId).OrderByDescending(i => i.InjuryDate).ToListAsync());
    }
}
