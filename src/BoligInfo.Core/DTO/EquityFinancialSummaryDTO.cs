namespace BoligInfo.Core.DTO;

public class EquityFinancialSummaryDto
{
    public long EquityId { get; set; }
    public int NumberOfHouses { get; set; }
    public double TotalMonthlyIncome { get; set; }
    public double TotalMonthlyExpenses { get; set; }
    public double NetMonthlyCashFlow { get; set; }
}
