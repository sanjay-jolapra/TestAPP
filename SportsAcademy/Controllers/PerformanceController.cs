using SportsAcademy.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsAcademy.Data;
using SportsAcademy.Models;

namespace SportsAcademy.Controllers;


[RequirePermission(AppModules.Performance)]
public class PerformanceController : Controller
{
    private readonly ApplicationDbContext _db;
    public PerformanceController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var students = await _db.Students.Include(s => s.Batch).OrderBy(s => s.Name).ToListAsync();
        var scores = new List<AthleteScoreVM>();
        foreach (var s in students)
            scores.Add(await CalculateScore(s));
        scores = scores.OrderByDescending(x => x.OverallScore).ToList();
        return View(scores);
    }

    public async Task<IActionResult> StudentReport(int id)
    {
        var student = await _db.Students.Include(s => s.Batch).FirstOrDefaultAsync(s => s.Id == id);
        if (student == null) return NotFound();
        var score = await CalculateScore(student);
        return View(score);
    }

    private async Task<AthleteScoreVM> CalculateScore(Student student)
    {
        var sid = student.Id;
        var now = DateTime.Today;

        double fitnessScore = await GetAvgScore(_db.FitnessTestResults
            .Where(r => r.StudentId == sid && r.TestDate >= now.AddMonths(-3)));

        double movementScore = await GetAvgScoreM(_db.MovementScreeningResults
            .Where(r => r.StudentId == sid && r.TestDate >= now.AddMonths(-3)));

        double skillScore = await GetAvgScoreS(_db.SkillAssessmentResults
            .Where(r => r.StudentId == sid && r.AssessmentDate >= now.AddMonths(-3)));

        double scScore = await GetAvgScoreSC(_db.SCAssessmentResults
            .Where(r => r.StudentId == sid && r.AssessmentDate >= now.AddMonths(-3)));

        double mentalScore = await GetAvgScoreMT(_db.MentalToughnessResults
            .Where(r => r.StudentId == sid && r.AssessmentDate >= now.AddMonths(-3)));

        double teamworkScore = await GetAvgScoreTW(_db.TeamworkResults
            .Where(r => r.StudentId == sid && r.AssessmentDate >= now.AddMonths(-3)));

        var overall = fitnessScore * 0.20 + movementScore * 0.15 + skillScore * 0.20
                    + scScore * 0.15 + mentalScore * 0.15 + teamworkScore * 0.15;

        var attendanceRate = await GetAttendanceRate(sid);
        overall = overall * 0.90 + attendanceRate * 0.10;

        return new AthleteScoreVM
        {
            Student = student,
            FitnessScore = Math.Round(fitnessScore, 1),
            MovementScore = Math.Round(movementScore, 1),
            SkillScore = Math.Round(skillScore, 1),
            SCScore = Math.Round(scScore, 1),
            MentalScore = Math.Round(mentalScore, 1),
            TeamworkScore = Math.Round(teamworkScore, 1),
            AttendanceRate = Math.Round(attendanceRate, 1),
            OverallScore = Math.Round(overall, 1)
        };
    }

    private async Task<double> GetAttendanceRate(int studentId)
    {
        var total = await _db.AttendanceRecords.CountAsync(a => a.StudentId == studentId);
        if (total == 0) return 0;
        var present = await _db.AttendanceRecords.CountAsync(a => a.StudentId == studentId && a.Status == AttendanceStatus.Present);
        return (double)present / total * 10;
    }

    private async Task<double> GetAvgScore(IQueryable<FitnessTestResult> q)
    {
        var vals = await q.Select(r => r.Value).ToListAsync();
        if (!vals.Any()) return 0;
        var nums = vals.Select(v => double.TryParse(v, out var d) ? d : 0).ToList();
        var max = nums.Max();
        if (max == 0) return 0;
        return nums.Average() / max * 10;
    }

    private async Task<double> GetAvgScoreM(IQueryable<MovementScreeningResult> q)
    {
        var scores = await q.Where(r => r.Score.HasValue).Select(r => r.Score!.Value).ToListAsync();
        if (!scores.Any()) return 0;
        return scores.Average();
    }

    private async Task<double> GetAvgScoreS(IQueryable<SkillAssessmentResult> q)
    {
        var scores = await q.Where(r => r.Score.HasValue).Select(r => r.Score!.Value).ToListAsync();
        if (!scores.Any()) return 0;
        return scores.Average();
    }

    private async Task<double> GetAvgScoreSC(IQueryable<SCAssessmentResult> q)
    {
        var scores = await q.Where(r => r.Score.HasValue).Select(r => r.Score!.Value).ToListAsync();
        if (!scores.Any()) return 0;
        return scores.Average();
    }

    private async Task<double> GetAvgScoreMT(IQueryable<MentalToughnessResult> q)
    {
        var data = await q.Include(r => r.MentalToughnessTest).ToListAsync();
        if (!data.Any()) return 0;
        return data.Average(r => r.MentalToughnessTest!.MaxScore > 0 ? (double)r.Score / r.MentalToughnessTest.MaxScore * 10 : 0);
    }

    private async Task<double> GetAvgScoreTW(IQueryable<TeamworkResult> q)
    {
        var data = await q.Include(r => r.TeamworkTest).ToListAsync();
        if (!data.Any()) return 0;
        return data.Average(r => r.TeamworkTest!.MaxScore > 0 ? (double)r.Score / r.TeamworkTest.MaxScore * 10 : 0);
    }
}


public class AthleteScoreVM
{
    public Student Student { get; set; } = null!;
    public double FitnessScore { get; set; }
    public double MovementScore { get; set; }
    public double SkillScore { get; set; }
    public double SCScore { get; set; }
    public double MentalScore { get; set; }
    public double TeamworkScore { get; set; }
    public double AttendanceRate { get; set; }
    public double OverallScore { get; set; }

    public string PerformanceTier => OverallScore switch
    {
        >= 8.5 => "Elite",
        >= 7 => "Advanced",
        >= 5 => "Developing",
        >= 3 => "Beginner",
        _ => "Insufficient Data"
    };

    public string TierColor => PerformanceTier switch
    {
        "Elite" => "success",
        "Advanced" => "primary",
        "Developing" => "warning",
        "Beginner" => "secondary",
        _ => "light"
    };
}
