namespace MonAnCuaEm;

/// <summary>Cấu hình kết nối Supabase — nơi lưu dữ liệu dùng chung trên mây.</summary>
public static class SupabaseConfig
{
    /// <summary>URL project (kết thúc bằng dấu '/').</summary>
    public const string Url = "https://arukeheigxkbhlsgqkrb.supabase.co/";

    /// <summary>Publishable key — được thiết kế để công khai trong trình duyệt.</summary>
    public const string Key = "sb_publishable_SeLcvVyIOTGa9Zf0YBYxmQ_C3aRkwEQ";

    /// <summary>"Mã gia đình" chung: ai vào cùng link (cùng mã) sẽ thấy chung dữ liệu.</summary>
    public const string Household = "baothy-a7f3c9e21b8d4056";
}
