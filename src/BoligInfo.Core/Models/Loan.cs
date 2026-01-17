using BoligInfo.Core.Enums;

namespace BoligInfo.Core.Models;

/// <summary>
/// Represents a loan associated with an equity.
/// Contains financial and structural information about the loan.
/// </summary>
public class Loan
{
    /// <summary>
    /// Unique identifier for the loan.
    /// </summary>
    public long Id { get; init; }
    
    /// <summary>
    /// Type of loan (e.g. fixed-rate, variable-rate).
    /// </summary>
    public LoanType? LoanType { get; set; }
    
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
    /// Foreign key reference to the associated equity.
    /// </summary>
    public long EquityId { get; set; }
}