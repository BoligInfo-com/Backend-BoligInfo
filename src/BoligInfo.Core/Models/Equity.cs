namespace BoligInfo.Core.Models;

/// <summary>
/// Represents equity within an investment.
/// Acts as the aggregate root for cash and associated loans.
/// </summary>
public class Equity
{
    /// <summary>
    /// Unique identifier for the equity.
    /// </summary>
    public long Id { get; init; }
    
    /// <summary>
    /// Currency in which the equity is denominated (e.g. DKK, EUR).
    /// </summary>
    public string? Currency { get; set; }
    
    /// <summary>
    /// Cash position associated with the equity.
    /// </summary>
    public Cash? Cash { get; set; }
    
    /// <summary>
    /// Collection of loans associated with the equity.
    /// </summary>
    public ICollection<Loan>? Loans { get; set; }
}