using BoligInfo.Core.Enums;

namespace BoligInfo.Core.Models;

public class Loan
{
    public int Id { get; set; }
    public LoanType LoanType { get; set; }
    public double LoanAmount { get; set; }
    public double InterestRate { get; set; }
    public int LoanLifetime { get; set; }
    public int EquityId { get; set; }
}