namespace MonAnCuaEm.Models;

/// <summary>Danh mục món ăn.</summary>
public enum MealCategory
{
    MainDish = 0,   // Món chính
    Soup = 1,       // Canh
    Dessert = 2,    // Tráng miệng
    Snack = 3       // Ăn vặt
}

/// <summary>Buổi ăn trong ngày.</summary>
public enum MealType
{
    Breakfast = 0,  // Sáng
    Lunch = 1,      // Trưa
    Dinner = 2      // Tối
}
