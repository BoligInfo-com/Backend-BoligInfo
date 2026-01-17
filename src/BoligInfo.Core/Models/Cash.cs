namespace BoligInfo.Core.Models;

/// <summary>
/// Represents liquid cash associated with an equity.
/// </summary>
public class Cash
{
    /// <summary>
    /// Unique identifier for the cash record.
    /// </summary>
    public long Id { get; init; }
    
    /// <summary>
    /// Amount of available cash.
    /// </summary>
    public double CashAmount { get; set; }
    
    /// <summary>
    /// Foreign key reference to the associated equity.
    /// </summary>
    public long EquityId { get; set; }
}