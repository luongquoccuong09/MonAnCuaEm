namespace MonAnCuaEm.Models;

/// <summary>Gán một món ăn vào một ô ngày/buổi trong lịch thực đơn.</summary>
public class MealPlanEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Ngày trong lịch.</summary>
    public DateOnly Date { get; set; }

    public MealType MealType { get; set; }

    /// <summary>Món ăn được gán (tham chiếu tới Recipe.Id).</summary>
    public Guid RecipeId { get; set; }
}
