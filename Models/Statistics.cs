namespace MonAnCuaEm.Models;

/// <summary>Một cột chi phí trên biểu đồ (nhãn + số tiền).</summary>
public record CostPoint(string Label, decimal Amount);

/// <summary>So sánh chi phí kỳ hiện tại với kỳ trước (tuần/tháng).</summary>
public class PeriodSummary
{
    public decimal Current { get; set; }
    public decimal Previous { get; set; }

    public decimal Delta => Current - Previous;

    /// <summary>% thay đổi so với kỳ trước. Null nếu kỳ trước = 0 mà kỳ này &gt; 0.</summary>
    public double? DeltaPercent
    {
        get
        {
            if (Previous == 0) return Current == 0 ? 0 : null;
            return (double)(Delta / Previous) * 100;
        }
    }
}
