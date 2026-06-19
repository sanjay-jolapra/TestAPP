using System.ComponentModel.DataAnnotations;

namespace SportsAcademy.Models;

public enum MealType { Breakfast, Lunch, Dinner, PreWorkout, PostWorkout, Snack }

public class NutritionLog
{
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Required]
    [Display(Name = "Meal Type")]
    public MealType MealType { get; set; }

    [StringLength(1000)]
    [Display(Name = "Food Items")]
    public string? FoodItems { get; set; }

    [Range(0, 10000)]
    public double? Calories { get; set; }

    [Range(0, 1000), Display(Name = "Protein (g)")]
    public double? Protein { get; set; }

    [Range(0, 1000), Display(Name = "Carbohydrates (g)")]
    public double? Carbs { get; set; }

    [Range(0, 1000), Display(Name = "Fats (g)")]
    public double? Fats { get; set; }

    [Range(0, 10000), Display(Name = "Water (ml)")]
    public double? WaterMl { get; set; }

    public string? Notes { get; set; }
}

public class NutritionGoal
{
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    [Display(Name = "Daily Calorie Goal")]
    public double? DailyCalories { get; set; }

    [Display(Name = "Protein Goal (g)")]
    public double? ProteinGoal { get; set; }

    [Display(Name = "Carbs Goal (g)")]
    public double? CarbsGoal { get; set; }

    [Display(Name = "Fats Goal (g)")]
    public double? FatsGoal { get; set; }

    [Display(Name = "Water Goal (ml)")]
    public double? WaterGoal { get; set; }

    public string? Notes { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Effective From")]
    public DateTime EffectiveFrom { get; set; } = DateTime.Today;
}
