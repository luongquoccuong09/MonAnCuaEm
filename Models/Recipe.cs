namespace MonAnCuaEm.Models;

/// <summary>Công thức một món ăn.</summary>
public class Recipe
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public MealCategory Category { get; set; } = MealCategory.MainDish;

    /// <summary>Ghi chú cách nấu.</summary>
    public string? Note { get; set; }

    /// <summary>Ghi chú kỷ niệm (ví dụ: "món này nấu hôm mình mới quen nhau").</summary>
    public string? Memory { get; set; }

    /// <summary>Ảnh món ăn: data URL base64 hoặc đường dẫn http.</summary>
    public string? ImageData { get; set; }

    public List<Ingredient> Ingredients { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>Tổng chi phí nguyên liệu của món (VND).</summary>
    public decimal TotalCost => Ingredients.Sum(i => i.Price);

    /// <summary>Bản sao sâu, dùng cho form sửa để không đụng vào dữ liệu gốc.</summary>
    public Recipe Clone() => new()
    {
        Id = Id,
        Name = Name,
        Category = Category,
        Note = Note,
        Memory = Memory,
        ImageData = ImageData,
        CreatedAt = CreatedAt,
        Ingredients = Ingredients
            .Select(i => new Ingredient { Name = i.Name, Price = i.Price })
            .ToList()
    };
}
