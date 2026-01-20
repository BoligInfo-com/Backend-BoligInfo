namespace BoligInfo.Core.DTO;

public class HouseFinancialSummaryDto
{
    public long HouseId { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public decimal NetMonthlyCashFlow { get; set; }
}