using System.Globalization;

namespace MonAnCuaEm.Helpers;

/// <summary>Định dạng hiển thị (tiền tệ, ngày).</summary>
public static class Format
{
    private static readonly CultureInfo Vi = CultureInfo.GetCultureInfo("vi-VN");

    /// <summary>Định dạng số tiền VND, ví dụ "125.000 ₫".</summary>
    public static string Money(decimal amount)
        => amount.ToString("#,##0", Vi) + " ₫";

    public static string ShortDate(DateOnly date)
        => date.ToString("dd/MM", Vi);

    public static string FullDate(DateOnly date)
        => date.ToString("dd/MM/yyyy", Vi);
}
