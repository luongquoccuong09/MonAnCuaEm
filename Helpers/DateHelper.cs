namespace MonAnCuaEm.Helpers;

/// <summary>Tính toán mốc tuần/tháng. Tuần bắt đầu từ Thứ 2.</summary>
public static class DateHelper
{
    /// <summary>Trả về ngày Thứ 2 của tuần chứa <paramref name="date"/>.</summary>
    public static DateOnly StartOfWeek(DateOnly date)
    {
        // DayOfWeek: Sunday = 0, Monday = 1 ... Saturday = 6.
        int diff = (7 + (int)date.DayOfWeek - (int)DayOfWeek.Monday) % 7;
        return date.AddDays(-diff);
    }

    /// <summary>7 ngày của tuần chứa <paramref name="date"/> (Thứ 2 -> Chủ nhật).</summary>
    public static List<DateOnly> WeekDays(DateOnly date)
    {
        var start = StartOfWeek(date);
        return Enumerable.Range(0, 7).Select(start.AddDays).ToList();
    }

    public static (DateOnly start, DateOnly end) WeekRange(DateOnly date)
    {
        var start = StartOfWeek(date);
        return (start, start.AddDays(6));
    }

    public static (DateOnly start, DateOnly end) MonthRange(DateOnly date)
    {
        var start = new DateOnly(date.Year, date.Month, 1);
        return (start, start.AddMonths(1).AddDays(-1));
    }

    public static bool InRange(DateOnly value, DateOnly start, DateOnly end)
        => value >= start && value <= end;
}
