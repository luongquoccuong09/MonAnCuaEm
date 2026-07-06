namespace MonAnCuaEm.Models;

/// <summary>Một nguyên liệu thuộc công thức món ăn.</summary>
public class Ingredient
{
    public string Name { get; set; } = string.Empty;

    /// <summary>Số lượng (ví dụ 200, 0.5...).</summary>
    public double Quantity { get; set; } = 1;

    /// <summary>Đơn vị (g, kg, quả, muỗng...).</summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>Giá tiền cho lượng nguyên liệu này (VND).</summary>
    public decimal Price { get; set; }
}
