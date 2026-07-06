using Blazored.LocalStorage;
using MonAnCuaEm.Models;

namespace MonAnCuaEm.Services;

/// <summary>Quản lý các khoản chi phí phát sinh nhập tay.</summary>
public class ExpenseService
{
    private const string Key = "monan.expenses";
    private readonly ILocalStorageService _storage;
    private List<ExtraExpense>? _cache;

    public event Action? Changed;

    public ExpenseService(ILocalStorageService storage) => _storage = storage;

    private async Task EnsureLoadedAsync()
    {
        if (_cache is not null) return;
        _cache = await _storage.GetItemAsync<List<ExtraExpense>>(Key) ?? new List<ExtraExpense>();
    }

    public async Task<List<ExtraExpense>> GetAllAsync()
    {
        await EnsureLoadedAsync();
        return _cache!.OrderByDescending(e => e.Date).ToList();
    }

    public async Task SaveAsync(ExtraExpense expense)
    {
        await EnsureLoadedAsync();
        var index = _cache!.FindIndex(e => e.Id == expense.Id);
        if (index >= 0) _cache[index] = expense;
        else _cache.Add(expense);
        await PersistAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await EnsureLoadedAsync();
        _cache!.RemoveAll(e => e.Id == id);
        await PersistAsync();
    }

    private async Task PersistAsync()
    {
        await _storage.SetItemAsync(Key, _cache);
        Changed?.Invoke();
    }
}
