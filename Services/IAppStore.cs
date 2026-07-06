namespace MonAnCuaEm.Services;

/// <summary>Kho lưu trữ khoá-giá trị dạng JSON cho dữ liệu app (LocalStorage hoặc Supabase).</summary>
public interface IAppStore
{
    Task<T?> GetItemAsync<T>(string key);
    Task SetItemAsync<T>(string key, T value);
}
