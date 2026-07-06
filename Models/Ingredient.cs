namespace MonAnCuaEm.Models;

/// <summary>Một nguyên liệu thuộc công thức món ăn.</summary>
public class Ingredient
{
    public string Name { get; set; } = string.Empty;

    /// <summary>Giá tiền cho lượng nguyên liệu này (VND).</summary>
    public decimal Price { get; set; }
}
