using SportsAcademy.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Models;

namespace SportsAcademy.Controllers;

[ModulePermission("Attendance")]
public class AttendanceController : Controller
{
    private readonly ApplicationDbContext _db;
    public AttendanceController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(DateTime? date, int? batchId)
    {
        date ??= DateTime.Today;
        ViewBag.Date = date.Value;
        ViewBag.Batches = new SelectList(await _db.Batches.ToListAsync(), "Id", "Name", batchId);
        ViewBag.SelectedBatch = batchId;

        var studentsQuery = _db.Students.Include(s => s.Batch).AsQueryable();
        if (batchId.HasValue)
            studentsQuery = studentsQuery.Where(s => s.BatchId == batchId);

        var students = await studentsQuery.OrderBy(s => s.Name).ToListAsync();
        var records = await _db.AttendanceRecords
            .Where(a => a.Date.Date == date.Value.Date)
            .ToListAsync();

        var vm = students.Select(s => new AttendanceVM
        {
            Student = s,
            Record = records.FirstOrDefault(r => r.StudentId == s.Id)
        }).ToList();

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> SaveBulk(List<int> studentIds, List<AttendanceStatus> statuses, DateTime date, int? batchId)
    {
        for (int i = 0; i < studentIds.Count; i++)
        {
            var existing = await _db.AttendanceRecords
                .FirstOrDefaultAsync(a => a.StudentId == studentIds[i] && a.Date.Date == date.Date);
            if (existing != null)
            {
                existing.Status = statuses[i];
                existing.BatchId = batchId;
            }
            else
            {
                _db.AttendanceRecords.Add(new AttendanceRecord
                {
                    StudentId = studentIds[i],
                    Date = date,
                    Status = statuses[i],
                    BatchId = batchId
                });
            }
        }
        await _db.SaveChangesAsync();
        TempData["Success"] = "Attendance saved!";
        return RedirectToAction(nameof(Index), new { date = date.ToString("yyyy-MM-dd"), batchId });
    }

    public async Task<IActionResult> Report(int? studentId, int? month, int? year)
    {
        year ??= DateTime.Today.Year;
        month ??= DateTime.Today.Month;
        ViewBag.Students = new SelectList(await _db.Students.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", studentId);
        ViewBag.Month = month;
        ViewBag.Year = year;

        if (!studentId.HasValue) return View(new List<AttendanceRecord>());

        var records = await _db.AttendanceRecords
            .Where(a => a.StudentId == studentId && a.Date.Month == month && a.Date.Year == year)
            .Include(a => a.Student)
            .OrderBy(a => a.Date)
            .ToListAsync();

        ViewBag.Student = await _db.Students.FindAsync(studentId);
        ViewBag.PresentCount = records.Count(r => r.Status == AttendanceStatus.Present);
        ViewBag.AbsentCount = records.Count(r => r.Status == AttendanceStatus.Absent);
        ViewBag.LateCount = records.Count(r => r.Status == AttendanceStatus.Late);
        return View(records);
    }
}

public class AttendanceVM
{
    public Student Student { get; set; } = null!;
    public AttendanceRecord? Record { get; set; }
}
