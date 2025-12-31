namespace BoligInfo.Core.DTOs;

public class LoanDto
{
    public long Id { get; set; }
    public string LoanType { get; set; }
    public double LoanAmount { get; set; }
    public double InterestRate { get; set; }
    public int LoanLifetime { get; set; }
    public long EquityId { get; set; }
}

public class CreateLoanDto
{
    public string LoanType { get; set; }
    public double LoanAmount { get; set; }
    public double InterestRate { get; set; }
    public int LoanLifetime { get; set; }
    public long EquityId { get; set; }
}

public class UpdateLoanDto
{
    public string? LoanType { get; set; }
    public double? LoanAmount { get; set; }
    public double? InterestRate { get; set; }
    public int? LoanLifetime { get; set; }
}