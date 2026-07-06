using Blazored.LocalStorage;
using MonAnCuaEm.Models;

namespace MonAnCuaEm.Services;

/// <summary>Lưu trữ và quản lý công thức món ăn (CRUD) trong LocalStorage.</summary>
public class RecipeService
{
    private const string Key = "monan.recipes";
    private readonly ILocalStorageService _storage;
    private List<Recipe>? _cache;

    public event Action? Changed;

    public RecipeService(ILocalStorageService storage) => _storage = storage;

    private async Task EnsureLoadedAsync()
    {
        if (_cache is not null) return;
        _cache = await _storage.GetItemAsync<List<Recipe>>(Key) ?? new List<Recipe>();
    }

    public async Task<List<Recipe>> GetAllAsync()
    {
        await EnsureLoadedAsync();
        return _cache!.OrderBy(r => r.Name).ToList();
    }

    public async Task<Recipe?> GetByIdAsync(Guid id)
    {
        await EnsureLoadedAsync();
        return _cache!.FirstOrDefault(r => r.Id == id);
    }

    public async Task SaveAsync(Recipe recipe)
    {
        await EnsureLoadedAsync();
        var index = _cache!.FindIndex(r => r.Id == recipe.Id);
        if (index >= 0) _cache[index] = recipe;
        else _cache.Add(recipe);
        await PersistAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await EnsureLoadedAsync();
        _cache!.RemoveAll(r => r.Id == id);
        await PersistAsync();
    }

    private async Task PersistAsync()
    {
        await _storage.SetItemAsync(Key, _cache);
        Changed?.Invoke();
    }
}
