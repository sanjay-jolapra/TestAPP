using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Models;

namespace SportsAcademy.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

    public HomeController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalStudents = await _db.Students.CountAsync();
        ViewBag.ActiveInjuries = await _db.InjuryRecords.CountAsync(i => i.Status == InjuryStatus.Active);
        ViewBag.TodayAttendance = await _db.AttendanceRecords
            .CountAsync(a => a.Date.Date == DateTime.Today && a.Status == AttendanceStatus.Present);
        ViewBag.RecentPayments = await _db.FeePayments
            .OrderByDescending(f => f.PaidDate)
            .Take(5)
            .Include(f => f.Student)
            .ToListAsync();
        ViewBag.RecentInjuries = await _db.InjuryRecords
            .Where(i => i.Status == InjuryStatus.Active)
            .Include(i => i.Student)
            .Take(5)
            .ToListAsync();
        return View();
    }

    public IActionResult Error() => View();
}
