using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;

namespace MonAnCuaEm.Services;

/// <summary>Đăng nhập Supabase (email + mật khẩu). Giữ phiên trong LocalStorage, tự làm mới token.</summary>
public class AuthService
{
    private const string SessionKey = "monan.session";
    private static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };

    private readonly HttpClient _http;
    private readonly ILocalStorageService _local;
    private AuthSession? _session;

    public event Action? Changed;

    public AuthService(HttpClient http, ILocalStorageService local)
    {
        _http = http;
        _local = local;
    }

    public bool IsAuthenticated => _session is not null && !string.IsNullOrEmpty(_session.RefreshToken);
    public string? Email => _session?.Email;

    /// <summary>Nạp phiên đã lưu (nếu có) khi mở app.</summary>
    public async Task InitializeAsync()
    {
        _session = await _local.GetItemAsync<AuthSession>(SessionKey);
        Changed?.Invoke();
    }

    /// <summary>Đăng nhập. Trả về null nếu thành công, hoặc thông báo lỗi (tiếng Việt).</summary>
    public async Task<string?> LoginAsync(string email, string password)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("auth/v1/token?grant_type=password",
                new { email, password }, Json);
            if (!resp.IsSuccessStatusCode)
                return await ExtractErrorAsync(resp);

            await StoreAsync(await resp.Content.ReadFromJsonAsync<TokenResponse>(Json));
            return null;
        }
        catch (Exception ex)
        {
            return "Không kết nối được máy chủ: " + ex.Message;
        }
    }

    public async Task LogoutAsync()
    {
        _session = null;
        await _local.RemoveItemAsync(SessionKey);
        Changed?.Invoke();
    }

    /// <summary>Token còn hạn để gọi API; tự làm mới nếu sắp hết hạn.</summary>
    public async Task<string?> GetAccessTokenAsync()
    {
        if (_session is null) return null;
        if (DateTime.UtcNow >= _session.ExpiresAt.AddSeconds(-60))
        {
            if (!await RefreshAsync()) return null;
        }
        return _session?.AccessToken;
    }

    private async Task<bool> RefreshAsync()
    {
        if (string.IsNullOrEmpty(_session?.RefreshToken)) return false;
        try
        {
            var resp = await _http.PostAsJsonAsync("auth/v1/token?grant_type=refresh_token",
                new { refresh_token = _session.RefreshToken }, Json);
            if (!resp.IsSuccessStatusCode)
            {
                await LogoutAsync();
                return false;
            }
            await StoreAsync(await resp.Content.ReadFromJsonAsync<TokenResponse>(Json));
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task StoreAsync(TokenResponse? t)
    {
        if (t is null || string.IsNullOrEmpty(t.AccessToken)) return;
        _session = new AuthSession
        {
            AccessToken = t.AccessToken,
            RefreshToken = t.RefreshToken ?? _session?.RefreshToken ?? "",
            ExpiresAt = DateTime.UtcNow.AddSeconds(t.ExpiresIn > 0 ? t.ExpiresIn : 3600),
            Email = t.User?.Email ?? _session?.Email ?? ""
        };
        await _local.SetItemAsync(SessionKey, _session);
        Changed?.Invoke();
    }

    private static async Task<string> ExtractErrorAsync(HttpResponseMessage resp)
    {
        try
        {
            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var root = doc.RootElement;
            foreach (var prop in new[] { "error_description", "msg", "error", "message" })
            {
                if (root.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String)
                    return Translate(v.GetString()!);
            }
        }
        catch { /* ignore */ }
        return "Đăng nhập thất bại.";
    }

    private static string Translate(string msg)
    {
        if (msg.Contains("Invalid login", StringComparison.OrdinalIgnoreCase) ||
            msg.Contains("invalid_credentials", StringComparison.OrdinalIgnoreCase))
            return "Sai email hoặc mật khẩu.";
        if (msg.Contains("Email not confirmed", StringComparison.OrdinalIgnoreCase))
            return "Tài khoản chưa được xác nhận.";
        return msg;
    }

    private class TokenResponse
    {
        [JsonPropertyName("access_token")] public string? AccessToken { get; set; }
        [JsonPropertyName("refresh_token")] public string? RefreshToken { get; set; }
        [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
        [JsonPropertyName("user")] public UserInfo? User { get; set; }
    }

    private class UserInfo
    {
        [JsonPropertyName("email")] public string? Email { get; set; }
    }
}

/// <summary>Phiên đăng nhập được lưu trên máy.</summary>
public class AuthSession
{
    public string AccessToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public string Email { get; set; } = "";
}
