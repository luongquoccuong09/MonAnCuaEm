namespace MonAnCuaEm.Models;

/// <summary>Chi phí phát sinh nhập tay (ngoài chi phí nguyên liệu trong lịch).</summary>
public class ExtraExpense
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateOnly Date { get; set; }

    /// <summary>Số tiền (VND).</summary>
    public decimal Amount { get; set; }

    public string? Note { get; set; }
}
