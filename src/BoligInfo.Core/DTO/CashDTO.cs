
namespace BoligInfo.Core.DTO;

/// <summary>
/// Data transfer object representing cash information.
/// </summary>
public class CashDto
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
    /// Identifier of the associated equity.
    /// </summary>
    public long EquityId { get; set; }
}

/// <summary>
/// DTO used when creating a new cash record.
/// </summary>
public class CreateCashDto
{
    /// <summary>
    /// Initial cash amount.
    /// </summary>
    public double CashAmount { get; set; }
    
    /// <summary>
    /// Identifier of the associated equity.
    /// </summary>
    public long EquityId { get; set; }
}

/// <summary>
/// DTO used when updating an existing cash record.
/// Only provided values will be updated.
/// </summary>
public class UpdateCashDto
{
    /// <summary>
    /// Updated cash amount.
    /// </summary>
    public double? CashAmount { get; set; }
}