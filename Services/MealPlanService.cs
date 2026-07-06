using MonAnCuaEm.Helpers;
using MonAnCuaEm.Models;

namespace MonAnCuaEm.Services;

/// <summary>Quản lý lịch thực đơn: gán/bỏ món vào từng ô ngày-buổi.</summary>
public class MealPlanService
{
    private const string Key = "monan.mealplan";
    private readonly IAppStore _storage;
    private List<MealPlanEntry>? _cache;

    public event Action? Changed;

    public MealPlanService(IAppStore storage) => _storage = storage;

    private async Task EnsureLoadedAsync()
    {
        if (_cache is not null) return;
        _cache = await _storage.GetItemAsync<List<MealPlanEntry>>(Key) ?? new List<MealPlanEntry>();
    }

    public async Task<List<MealPlanEntry>> GetAllAsync()
    {
        await EnsureLoadedAsync();
        return _cache!;
    }

    public async Task<List<MealPlanEntry>> GetWeekAsync(DateOnly anyDayInWeek)
    {
        await EnsureLoadedAsync();
        var (start, end) = DateHelper.WeekRange(anyDayInWeek);
        return _cache!.Where(e => DateHelper.InRange(e.Date, start, end)).ToList();
    }

    public async Task<MealPlanEntry?> GetEntryAsync(DateOnly date, MealType meal)
    {
        await EnsureLoadedAsync();
        return _cache!.FirstOrDefault(e => e.Date == date && e.MealType == meal);
    }

    public async Task AssignAsync(DateOnly date, MealType meal, Guid recipeId)
    {
        await EnsureLoadedAsync();
        var existing = _cache!.FirstOrDefault(e => e.Date == date && e.MealType == meal);
        if (existing is null)
            _cache!.Add(new MealPlanEntry { Date = date, MealType = meal, RecipeId = recipeId });
        else
            existing.RecipeId = recipeId;
        await PersistAsync();
    }

    public async Task ClearAsync(DateOnly date, MealType meal)
    {
        await EnsureLoadedAsync();
        _cache!.RemoveAll(e => e.Date == date && e.MealType == meal);
        await PersistAsync();
    }

    /// <summary>Xoá mọi ô đang trỏ tới một món (dùng khi xoá món khỏi thư viện).</summary>
    public async Task RemoveRecipeAsync(Guid recipeId)
    {
        await EnsureLoadedAsync();
        var removed = _cache!.RemoveAll(e => e.RecipeId == recipeId);
        if (removed > 0) await PersistAsync();
    }

    private async Task PersistAsync()
    {
        await _storage.SetItemAsync(Key, _cache);
        Changed?.Invoke();
    }
}
