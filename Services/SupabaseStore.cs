using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using MonAnCuaEm.Models;

namespace MonAnCuaEm.Services;

/// <summary>
/// Lưu dữ liệu lên Supabase (bảng app_data), dùng token của người đăng nhập (bảo vệ bằng RLS).
/// Mirror xuống LocalStorage để vẫn xem được khi offline, và tự chuyển dữ liệu cũ trên máy
/// lên mây ở lần chạy đầu tiên.
/// </summary>
public class SupabaseStore : IAppStore
{
    private const string MigratedFlag = "monan.cloud.migrated";
    private static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };

    private readonly HttpClient _http;
    private readonly ILocalStorageService _local;
    private readonly AuthService _auth;
    private Task? _migration;

    public SupabaseStore(HttpClient http, ILocalStorageService local, AuthService auth)
    {
        _http = http;
        _local = local;
        _auth = auth;
    }

    public async Task<T?> GetItemAsync<T>(string key)
    {
        await EnsureMigratedAsync();
        try
        {
            var value = await GetCloudAsync<T>(key);
            if (value is not null) await _local.SetItemAsync(key, value); // mirror để xem offline
            return value;
        }
        catch
        {
            return await _local.GetItemAsync<T>(key);
        }
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        await EnsureMigratedAsync();
        await _local.SetItemAsync(key, value); // luôn giữ mirror trên máy
        await SetCloudAsync(key, value);
    }

    // ---------- Chuyển dữ liệu cũ trên máy lên mây (chạy 1 lần) ----------

    private Task EnsureMigratedAsync() => _migration ??= MigrateAsync();

    private async Task MigrateAsync()
    {
        try
        {
            if (await _local.GetItemAsync<bool>(MigratedFlag)) return;
            await MigrateKeyAsync<List<Recipe>>("monan.recipes");
            await MigrateKeyAsync<List<MealPlanEntry>>("monan.mealplan");
            await MigrateKeyAsync<List<ExtraExpense>>("monan.expenses");
            await _local.SetItemAsync(MigratedFlag, true);
        }
        catch
        {
            // Best-effort: lỗi thì bỏ qua, lần sau thử lại.
        }
    }

    private async Task MigrateKeyAsync<T>(string key)
    {
        if (await GetCloudAsync<T>(key) is not null) return; // mây đã có -> tôn trọng mây
        var local = await _local.GetItemAsync<T>(key);
        if (local is not null) await SetCloudAsync(key, local);
    }

    // ---------- REST tới Supabase (PostgREST), kèm token đăng nhập ----------

    private async Task<T?> GetCloudAsync<T>(string key)
    {
        var uri = $"rest/v1/app_data?household=eq.{SupabaseConfig.Household}" +
                  $"&doc_type=eq.{Uri.EscapeDataString(key)}&select=data";
        using var req = await BuildRequestAsync(HttpMethod.Get, uri);
        using var resp = await _http.SendAsync(req);
        resp.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
        if (doc.RootElement.ValueKind != JsonValueKind.Array || doc.RootElement.GetArrayLength() == 0)
            return default;

        return doc.RootElement[0].GetProperty("data").Deserialize<T>(Json);
    }

    private async Task SetCloudAsync<T>(string key, T value)
    {
        var payload = new Dictionary<string, object?>
        {
            ["household"] = SupabaseConfig.Household,
            ["doc_type"] = key,
            ["data"] = value,
            ["updated_at"] = DateTime.UtcNow
        };

        using var req = await BuildRequestAsync(HttpMethod.Post, "rest/v1/app_data");
        req.Content = JsonContent.Create(payload, options: Json);
        req.Headers.TryAddWithoutValidation("Prefer", "resolution=merge-duplicates");

        using var resp = await _http.SendAsync(req);
        resp.EnsureSuccessStatusCode();
    }

    private async Task<HttpRequestMessage> BuildRequestAsync(HttpMethod method, string uri)
    {
        var req = new HttpRequestMessage(method, uri);
        var token = await _auth.GetAccessTokenAsync();
        if (!string.IsNullOrEmpty(token))
            req.Headers.TryAddWithoutValidation("Authorization", $"Bearer {token}");
        return req;
    }
}
