using MonAnCuaEm.Helpers;
using MonAnCuaEm.Models;

namespace MonAnCuaEm.Services;

/// <summary>Tổng hợp chi phí: cộng chi phí nguyên liệu các món trong lịch với chi phí phát sinh.</summary>
public class StatisticsService
{
    private readonly RecipeService _recipes;
    private readonly MealPlanService _plan;
    private readonly ExpenseService _expenses;

    public StatisticsService(RecipeService recipes, MealPlanService plan, ExpenseService expenses)
    {
        _recipes = recipes;
        _plan = plan;
        _expenses = expenses;
    }

    /// <summary>Tổng chi phí trong khoảng ngày [start, end].</summary>
    public async Task<decimal> CostInRangeAsync(DateOnly start, DateOnly end)
    {
        var costById = (await _recipes.GetAllAsync()).ToDictionary(r => r.Id, r => r.TotalCost);

        var planCost = (await _plan.GetAllAsync())
            .Where(e => DateHelper.InRange(e.Date, start, end) && costById.ContainsKey(e.RecipeId))
            .Sum(e => costById[e.RecipeId]);

        var extraCost = (await _expenses.GetAllAsync())
            .Where(x => DateHelper.InRange(x.Date, start, end))
            .Sum(x => x.Amount);

        return planCost + extraCost;
    }

    /// <summary>Chuỗi chi phí <paramref name="weeks"/> tuần gần nhất (tính tới tuần chứa <paramref name="reference"/>).</summary>
    public async Task<List<CostPoint>> WeeklySeriesAsync(int weeks, DateOnly reference)
    {
        var result = new List<CostPoint>();
        for (int i = weeks - 1; i >= 0; i--)
        {
            var day = reference.AddDays(-7 * i);
            var (start, end) = DateHelper.WeekRange(day);
            var amount = await CostInRangeAsync(start, end);
            result.Add(new CostPoint(Format.ShortDate(start), amount));
        }
        return result;
    }

    /// <summary>Chuỗi chi phí <paramref name="months"/> tháng gần nhất.</summary>
    public async Task<List<CostPoint>> MonthlySeriesAsync(int months, DateOnly reference)
    {
        var result = new List<CostPoint>();
        for (int i = months - 1; i >= 0; i--)
        {
            var day = reference.AddMonths(-i);
            var (start, end) = DateHelper.MonthRange(day);
            var amount = await CostInRangeAsync(start, end);
            result.Add(new CostPoint($"{start.Month:00}/{start.Year}", amount));
        }
        return result;
    }

    public async Task<PeriodSummary> WeekSummaryAsync(DateOnly reference)
    {
        var (start, end) = DateHelper.WeekRange(reference);
        var (prevStart, prevEnd) = DateHelper.WeekRange(reference.AddDays(-7));
        return new PeriodSummary
        {
            Current = await CostInRangeAsync(start, end),
            Previous = await CostInRangeAsync(prevStart, prevEnd)
        };
    }

    public async Task<PeriodSummary> MonthSummaryAsync(DateOnly reference)
    {
        var (start, end) = DateHelper.MonthRange(reference);
        var (prevStart, prevEnd) = DateHelper.MonthRange(reference.AddMonths(-1));
        return new PeriodSummary
        {
            Current = await CostInRangeAsync(start, end),
            Previous = await CostInRangeAsync(prevStart, prevEnd)
        };
    }
}
