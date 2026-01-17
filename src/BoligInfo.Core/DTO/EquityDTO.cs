using BoligInfo.Core.Models;

namespace BoligInfo.Core.DTO;

/// <summary>
/// Data transfer object representing equity data.
/// </summary>
public class EquityDto
{
    /// <summary>
    /// Unique identifier for the equity.
    /// </summary>
    public long Id { get; init; }
    
    /// <summary>
    /// Currency in which the equity is denominated.
    /// </summary>
    public string? Currency { get; set; }
    
    /// <summary>
    /// Cash position associated with the equity.
    /// </summary>
    public Cash? Cash { get; set; }
    
    /// <summary>
    /// Loans associated with the equity.
    /// </summary>
    public ICollection<LoanDto>? Loans { get; set; }
}

/// <summary>
/// DTO used when creating a new equity.
/// </summary>
public class CreateEquityDto
{
    /// <summary>
    /// Currency of the equity.
    /// </summary>
    public string? Currency { get; set; }
}

/// <summary>
/// DTO used when updating an existing equity.
/// Only provided values will be updated.
/// </summary>
public class UpdateEquityDto
{
    /// <summary>
    /// Updated currency.
    /// </summary>
    public string? Currency { get; set; }
}