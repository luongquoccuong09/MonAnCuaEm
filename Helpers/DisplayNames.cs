using MonAnCuaEm.Models;

namespace MonAnCuaEm.Helpers;

/// <summary>Nhãn tiếng Việt cho các enum và ngày trong tuần.</summary>
public static class DisplayNames
{
    public static string ToVietnamese(this MealCategory category) => category switch
    {
        MealCategory.MainDish => "Món chính",
        MealCategory.Soup => "Canh",
        MealCategory.Dessert => "Tráng miệng",
        MealCategory.Snack => "Ăn vặt",
        _ => category.ToString()
    };

    public static string ToVietnamese(this MealType meal) => meal switch
    {
        MealType.Breakfast => "Sáng",
        MealType.Lunch => "Trưa",
        MealType.Dinner => "Tối",
        _ => meal.ToString()
    };

    /// <summary>Thứ trong tuần bằng tiếng Việt (Thứ 2 ... Chủ nhật).</summary>
    public static string WeekdayVi(this DateOnly date) => date.DayOfWeek switch
    {
        DayOfWeek.Monday => "Thứ 2",
        DayOfWeek.Tuesday => "Thứ 3",
        DayOfWeek.Wednesday => "Thứ 4",
        DayOfWeek.Thursday => "Thứ 5",
        DayOfWeek.Friday => "Thứ 6",
        DayOfWeek.Saturday => "Thứ 7",
        DayOfWeek.Sunday => "Chủ nhật",
        _ => string.Empty
    };

    public static readonly MealCategory[] AllCategories =
        { MealCategory.MainDish, MealCategory.Soup, MealCategory.Dessert, MealCategory.Snack };

    public static readonly MealType[] AllMealTypes =
        { MealType.Breakfast, MealType.Lunch, MealType.Dinner };
}
