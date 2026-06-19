using SportsAcademy.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Models;

namespace SportsAcademy.Controllers;


[RequirePermission(AppModules.Fees)]
public class FeesController : Controller
{
    private readonly ApplicationDbContext _db;
    public FeesController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(int? studentId, int? year)
    {
        year ??= DateTime.Today.Year;
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", studentId);
        ViewBag.Year = year;

        var query = _db.FeePayments.Include(f => f.Student).AsQueryable();
        if (studentId.HasValue) query = query.Where(f => f.StudentId == studentId);
        if (year.HasValue) query = query.Where(f => f.PeriodYear == year || f.PaidDate.Year == year);

        var payments = await query.OrderByDescending(f => f.PaidDate).ToListAsync();
        ViewBag.TotalAmount = payments.Sum(p => p.Amount);
        return View(payments);
    }

    public async Task<IActionResult> Create(int? studentId)
    {
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", studentId);
        return View(new FeePayment { PaidDate = DateTime.Today, PeriodYear = DateTime.Today.Year, PeriodMonth = DateTime.Today.Month });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FeePayment fee)
    {
        if (ModelState.IsValid)
        {
            _db.FeePayments.Add(fee);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Fee payment recorded!";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", fee.StudentId);
        return View(fee);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var fee = await _db.FeePayments.FindAsync(id);
        if (fee == null) return NotFound();
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", fee.StudentId);
        return View(fee);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FeePayment fee)
    {
        if (id != fee.Id) return BadRequest();
        if (ModelState.IsValid)
        {
            _db.FeePayments.Update(fee);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Payment updated!";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", fee.StudentId);
        return View(fee);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var fee = await _db.FeePayments.FindAsync(id);
        if (fee != null) { _db.FeePayments.Remove(fee); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Payment deleted.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> StudentLedger(int studentId)
    {
        var student = await _db.Students.FindAsync(studentId);
        if (student == null) return NotFound();
        var payments = await _db.FeePayments
            .Where(f => f.StudentId == studentId)
            .OrderByDescending(f => f.PaidDate)
            .ToListAsync();
        ViewBag.Student = student;
        ViewBag.Total = payments.Sum(p => p.Amount);
        return View(payments);
    }
}
