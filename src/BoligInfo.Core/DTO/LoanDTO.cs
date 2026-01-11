using System.ComponentModel.DataAnnotations;

namespace BoligInfo.Core.DTO;

public class LoanDto
{
    public long Id { get; init; }
    public string? LoanType { get; set; }
    public double LoanAmount { get; set; }
    public double InterestRate { get; set; }
    public int? LoanLifetime { get; set; }
    public long EquityId { get; set; }
}

public class CreateLoanDto
{
    public string? LoanType { get; set; }
    [Range(0, double.MaxValue, ErrorMessage = "Loan amount must be zero or positive")]
    public double LoanAmount { get; set; }
    public double InterestRate { get; set; }
    public int LoanLifetime { get; set; }
    public long EquityId { get; set; }
}

public class UpdateLoanDto
{
    public string? LoanType { get; set; }
    [Range(0, double.MaxValue, ErrorMessage = "Loan amount must be zero or positive")]
    public double? LoanAmount { get; set; }
    public double? InterestRate { get; set; }
    public int? LoanLifetime { get; set; }
}