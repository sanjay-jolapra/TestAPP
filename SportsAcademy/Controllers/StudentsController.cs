using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Models;

namespace SportsAcademy.Controllers;

public class StudentsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public StudentsController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var students = await _db.Students.Include(s => s.Batch).OrderBy(s => s.Name).ToListAsync();
        return View(students);
    }

    public async Task<IActionResult> Details(int id)
    {
        var student = await _db.Students
            .Include(s => s.Batch)
            .Include(s => s.AttendanceRecords)
            .Include(s => s.FeePayments)
            .Include(s => s.InjuryRecords)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (student == null) return NotFound();
        return View(student);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Batches = new SelectList(await _db.Batches.ToListAsync(), "Id", "Name");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Student student, IFormFile? photo)
    {
        if (ModelState.IsValid)
        {
            student.StudentCode = GenerateStudentCode();
            if (photo != null && photo.Length > 0)
                student.PhotoPath = await SavePhoto(photo);
            _db.Students.Add(student);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Student added successfully!";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Batches = new SelectList(await _db.Batches.ToListAsync(), "Id", "Name");
        return View(student);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var student = await _db.Students.FindAsync(id);
        if (student == null) return NotFound();
        ViewBag.Batches = new SelectList(await _db.Batches.ToListAsync(), "Id", "Name", student.BatchId);
        return View(student);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Student student, IFormFile? photo)
    {
        if (id != student.Id) return BadRequest();
        if (ModelState.IsValid)
        {
            var existing = await _db.Students.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Name = student.Name;
            existing.Phone = student.Phone;
            existing.ParentName = student.ParentName;
            existing.ParentPhone = student.ParentPhone;
            existing.DateOfBirth = student.DateOfBirth;
            existing.BloodGroup = student.BloodGroup;
            existing.SchoolCollege = student.SchoolCollege;
            existing.Address = student.Address;
            existing.JerseySize = student.JerseySize;
            existing.JoiningDate = student.JoiningDate;
            existing.BatchId = student.BatchId;
            if (photo != null && photo.Length > 0)
                existing.PhotoPath = await SavePhoto(photo);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Student updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Batches = new SelectList(await _db.Batches.ToListAsync(), "Id", "Name", student.BatchId);
        return View(student);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var student = await _db.Students.FindAsync(id);
        if (student != null)
        {
            _db.Students.Remove(student);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Student deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    private string GenerateStudentCode()
    {
        var count = _db.Students.Count() + 1;
        return $"SA{DateTime.Now.Year}{count:D4}";
    }

    private async Task<string> SavePhoto(IFormFile photo)
    {
        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "photos");
        Directory.CreateDirectory(uploadsDir);
        var ext = Path.GetExtension(photo.FileName);
        var fileName = $"{Guid.NewGuid()}{ext}";
        var path = Path.Combine(uploadsDir, fileName);
        using var stream = new FileStream(path, FileMode.Create);
        await photo.CopyToAsync(stream);
        return $"/uploads/photos/{fileName}";
    }
}
