using System.ComponentModel.DataAnnotations;

namespace BoligInfo.Core.DTO;

/// <summary>
/// Data transfer object representing loan data.
/// </summary>
public class LoanDto
{
    /// <summary>
    /// Unique identifier for the loan.
    /// </summary>
    public long Id { get; init; }
    
    /// <summary>
    /// Type of loan.
    /// </summary>
    public string? LoanType { get; set; }
    
    /// <summary>
    /// Principal amount of the loan.
    /// </summary>
    public double LoanAmount { get; set; }
    
    /// <summary>
    /// Interest rate applied to the loan.
    /// </summary>
    public double InterestRate { get; set; }
    
    /// <summary>
    /// Loan duration in years.
    /// </summary>
    public int? LoanLifetime { get; set; }
    
    /// <summary>
    /// Identifier of the associated equity.
    /// </summary>
    public long EquityId { get; set; }
}

/// <summary>
/// DTO used when creating a new loan.
/// Includes validation rules for input data.
/// </summary>
public class CreateLoanDto
{
    /// <summary>
    /// Type of loan.
    /// </summary>
    public string? LoanType { get; set; }
    
    /// <summary>
    /// Principal amount of the loan.
    /// Must be zero or positive.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Loan amount must be zero or positive")]
    public double LoanAmount { get; set; }
    
    /// <summary>
    /// Interest rate applied to the loan.
    /// </summary>
    public double InterestRate { get; set; }
    
    /// <summary>
    /// Loan duration in years.
    /// Must be a positive value.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Loan lifetime must be positive")]
    public int LoanLifetime { get; set; }
    
    /// <summary>
    /// Identifier of the associated equity.
    /// </summary>
    public long EquityId { get; set; }
}

/// <summary>
/// DTO used when updating an existing loan.
/// Only provided values will be updated.
/// </summary>
public class UpdateLoanDto
{
    /// <summary>
    /// Updated loan type.
    /// </summary>
    public string? LoanType { get; set; }
    
    /// <summary>
    /// Updated loan amount.
    /// Must be zero or positive.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Loan amount must be zero or positive")]
    public double? LoanAmount { get; set; }
    
    /// <summary>
    /// Updated interest rate.
    /// </summary>
    public double? InterestRate { get; set; }
    
    /// <summary>
    /// Updated loan duration in years.
    /// Must be a positive value.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Loan lifetime must be positive")]
    public int? LoanLifetime { get; set; }
}