namespace BoligInfo.Core.DTO;

public class PortfolioSummaryDto
{
    public decimal TotalMonthlyCashFlow { get; set; }
    public decimal TotalAnnualCashFlow { get; set; }
    public int TotalProperties { get; set; }
}