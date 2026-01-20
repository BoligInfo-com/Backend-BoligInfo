namespace BoligInfo.Core.DTO;

public class EquityFinancialSummaryDto
{
    public long EquityId { get; set; }
    public int NumberOfHouses { get; set; }
    public decimal TotalMonthlyIncome { get; set; }
    public decimal TotalMonthlyExpenses { get; set; }
    public decimal NetMonthlyCashFlow { get; set; }
}
