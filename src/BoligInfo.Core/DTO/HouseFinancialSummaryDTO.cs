namespace BoligInfo.Core.DTO;

public class HouseFinancialSummaryDto
{
    public long HouseId { get; set; }
    public double MonthlyIncome { get; set; }
    public double MonthlyExpenses { get; set; }
    public double NetMonthlyCashFlow { get; set; }
}